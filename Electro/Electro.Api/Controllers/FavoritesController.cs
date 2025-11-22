using Electro.Core.Dtos;
using Electro.Core.Dtos.Favorite;
using Electro.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Electro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // لازم JWT صالح
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _service;

        public FavoritesController(IFavoriteService service) => _service = service;

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token: userId not found.");
            return userId;
        }

        // GET: api/favorites?pageNumber=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult> GetUserFavorites(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var userId = GetUserId();
            var paging = new PaginationParams { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _service.GetUserFavoritesAsync(userId, paging, ct);
            return Ok(new { statusCode = 200, message = "favorites", data = result });
        }

        // POST: api/favorites/{productId}
        [HttpPost("{productId:int}")]
        public async Task<ActionResult> Add(int productId, CancellationToken ct = default)
        {
            var userId = GetUserId();
            var ok = await _service.AddToFavoritesAsync(userId, productId, ct);
            if (!ok) return NotFound(new { statusCode = 404, message = "Product not found." });
            return Ok(new { statusCode = 200, message = "added to favorites" });
        }

        // DELETE: api/favorites/{productId}
        [HttpDelete("{productId:int}")]
        public async Task<ActionResult> Remove(int productId, CancellationToken ct = default)
        {
            var userId = GetUserId();
            var ok = await _service.RemoveFromFavoritesAsync(userId, productId, ct);
            if (!ok) return NotFound(new { statusCode = 404, message = "favorite not found" });
            return Ok(new { statusCode = 200, message = "removed from favorites" });
        }

        // GET: api/favorites/{productId}/is-favorite
        [HttpGet("{productId:int}/is-favorite")]
        public async Task<ActionResult> IsFavorite(int productId, CancellationToken ct = default)
        {
            var userId = GetUserId();
            var yes = await _service.IsInFavoritesAsync(userId, productId, ct);
            return Ok(new { statusCode = 200, message = "is favorite", data = yes });
        }
    }
}
