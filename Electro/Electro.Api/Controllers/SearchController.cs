using System.Security.Claims;
using Electro.Core.Dtos.Search;
using Electro.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Electro.Apis.Controllers
{
    // شيل [Authorize] من على مستوى الكنترولر علشان الإجراء يبقى متاح للعامة
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _search;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ISearchService search, ILogger<SearchController> logger)
        {
            _search = search;
            _logger = logger;
        }

        private string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
        private bool IsAdmin => User?.Identity?.IsAuthenticated == true && User.IsInRole("Admin");

        /// <summary>
        /// Central search endpoint (public for homepage; admin-only areas filtered automatically).
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // ✅ إتاحة الوصول بدون توكن
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        // (اختياري) كاش خفيف حسب البارامترات
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "q", "areas", "page", "pageSize" })]
        public async Task<IActionResult> Search(
            [FromQuery] string q,
            [FromQuery] string[]? areas,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            // ====== Basic validation ======
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(q))
                errors["q"] = new[] { "q is required." };
            else if (q.Trim().Length < 1)
                errors["q"] = new[] { "Query must be at least 1 characters." };

            if (page < 1) errors["page"] = new[] { "page must be >= 1." };
            if (pageSize < 1 || pageSize > 100) errors["pageSize"] = new[] { "pageSize must be between 1 and 100." };

            if (errors.Count > 0)
                return BadRequest(ApiValidationError(errors));

            // ====== هوية اختيارية ======
            var isAuthenticated = User?.Identity?.IsAuthenticated == true;
            var requesterId = isAuthenticated ? UserId : null;
            var requesterIsAdmin = isAuthenticated && IsAdmin;

            // ====== منع مناطق الـ admin لو مش admin ======
            string[]? safeAreas = areas;
            if (!requesterIsAdmin && areas != null && areas.Length > 0)
            {
                safeAreas = areas
                    .Where(a =>
                        !string.Equals(a, "orders", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(a, "users", StringComparison.OrdinalIgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (safeAreas.Length == 0) safeAreas = null; // سيطبّق الديفولت داخل الخدمة
            }

            try
            {
                var dto = new SearchQueryDto
                {
                    Q = q.Trim(),
                    Areas = safeAreas,
                    Page = page,
                    PageSize = Math.Clamp(pageSize, 1, 100)
                };

                var result = await _search.SearchAsync(dto, requesterId, requesterIsAdmin, ct);

                var adminAreasRequested = areas?.Any(a =>
                    string.Equals(a, "orders", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(a, "users", StringComparison.OrdinalIgnoreCase)) == true;

                return Ok(new
                {
                    statusCode = 200,
                    message = "search results",
                    data = result,
                    warning = (!requesterIsAdmin && adminAreasRequested)
                        ? "Admin-only areas (orders/users) were ignored for unauthenticated/normal users."
                        : null
                });
            }
            catch (ArgumentException ex) // زى "Query too short" من السيرفس
            {
                return BadRequest(ApiError(
                    code: "validation_error",
                    message: ex.Message,
                    errors: new Dictionary<string, string[]>
                    {
                        ["q"] = new[] { ex.Message }
                    }));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiError(
                    code: "forbidden",
                    message: "You are not allowed to access these results."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiError(
                    code: "not_found",
                    message: string.IsNullOrWhiteSpace(ex.Message) ? "Resource not found." : ex.Message));
            }
            catch (OperationCanceledException)
            {
                // 499: Client Closed Request (غير قياسي لكنه شائع)
                return StatusCode(499, ApiError(
                    code: "client_closed_request",
                    message: "The request was cancelled by the client."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in Search endpoint. q={Q}", q);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiError(
                    code: "server_error",
                    message: "Something went wrong. Please try again later."));
            }
        }

        // ===== Helpers for a consistent error envelope =====

        private object ApiError(string code, string message, Dictionary<string, string[]>? errors = null)
        {
            return new
            {
                statusCode = HttpContext.Response?.StatusCode == 0
                    ? StatusCodes.Status400BadRequest
                    : HttpContext.Response.StatusCode,
                error = code,
                message,
                errors,
                traceId = HttpContext.TraceIdentifier
            };
        }

        private object ApiValidationError(Dictionary<string, string[]> errors)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new
            {
                statusCode = StatusCodes.Status400BadRequest,
                error = "validation_error",
                message = "One or more validation errors occurred.",
                errors,
                traceId = HttpContext.TraceIdentifier
            };
        }
    }
}
