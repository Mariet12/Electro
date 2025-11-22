using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Electro.Core.Dtos.Notification;
using Electro.Core.Interface;
using Electro.Reposatory.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Electro.Apis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _svc;
        private readonly AppIdentityDbContext _db;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            INotificationService svc,
            AppIdentityDbContext db,
            ILogger<NotificationsController> logger)
        {
            _svc = svc;
            _db = db;
            _logger = logger;
        }



        private string GetUserIdFlexible()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("userId not found.");
            return userId;
        }

        // GET /api/notifications
        [HttpGet]
        public async Task<IActionResult> GetMy(
            [FromQuery] NotificationQueryDto? query,
            CancellationToken ct = default)
        {
            var userId = GetUserIdFlexible();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { statusCode = 401, message = "Unauthorized. Please login first." });

            try
            {
                var dto = await _svc.GetMyAsync(userId, query, ct);
                return Ok(new { statusCode = 200, message = "all data", data = dto });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new { statusCode = 499, message = "Client closed request." });
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid arguments in GetMy. traceId={TraceId}", HttpContext?.TraceIdentifier);
                return BadRequest(new { statusCode = 400, message = aex.Message, traceId = HttpContext?.TraceIdentifier });
            }
            catch (DbUpdateException dbex)
            {
                _logger.LogError(dbex, "Database error in GetMy. traceId={TraceId}", HttpContext?.TraceIdentifier);
                return StatusCode(503, new { statusCode = 503, message = "Database unavailable.", traceId = HttpContext?.TraceIdentifier });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMy failed. traceId={TraceId}, uid={UserId}", HttpContext?.TraceIdentifier, userId);
                return StatusCode(500, new { statusCode = 500, message = "Internal server error while fetching notifications.", traceId = HttpContext?.TraceIdentifier });
            }
        }

        // GET /api/notifications/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOne(int id, CancellationToken ct = default)
        {
            var userId = GetUserIdFlexible();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { statusCode = 401, message = "Unauthorized. Please login first." });

            var n = await _db.Notifications.AsNoTracking()
                .Where(x => x.Id == id && x.ReceiverId == userId)
                .Select(x => new NotificationListItemDto
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    ReceiverId = x.ReceiverId,
                    Title = x.Title,
                    Message = x.Message,
                    Status = x.Status,
                    OrderId = x.OrderId,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync(ct);

            return n is null
                ? NotFound(new { statusCode = 404, message = "Not found" })
                : Ok(new { statusCode = 200, message = "all data", data = n });
        }

        // POST /api/notifications/{id}/read
        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id, CancellationToken ct = default)
        {
            var userId = GetUserIdFlexible();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { statusCode = 401, message = "Unauthorized. Please login first." });

            var ok = await _svc.MarkAsReadAsync(id, userId);
            return ok
                ? Ok(new { statusCode = 200, message = "done", data = ok })
                : NotFound(new { statusCode = 404, message = "Not found" });
        }

        // DELETE /api/notifications/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var userId = GetUserIdFlexible();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { statusCode = 401, message = "Unauthorized. Please login first." });

            var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == userId, ct);
            if (n is null)
                return NotFound(new { statusCode = 404, message = "Not found" });

            _db.Notifications.Remove(n);
            await _db.SaveChangesAsync(ct);
            return Ok(new { statusCode = 200, message = "deleted" });
        }
        // POST /api/notifications/read-all
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead(CancellationToken ct = default)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { statusCode = 401, message = "Unauthorized. Please login first." });

            var count = await _svc.MarkAllAsReadAsync(userId, ct);

            return Ok(new
            {
                statusCode = 200,
                message = "all marked as read",
                affected = count
            });
        }

    }
}
