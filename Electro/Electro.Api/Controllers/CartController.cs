using Electro.Core.Dtos.Cart;
using Electro.Core.Interface;
using Electro.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Electro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // لازم JWT token
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service) => _service = service;

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token: userId not found.");
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult> GetCart(CancellationToken ct = default)
        {
            var userId = GetUserId();
            var cart = await _service.GetCartAsync(userId, ct);
            return Ok(new { statusCode = 200, message = "cart", data = cart });
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add([FromBody] AddToCartDto dto, CancellationToken ct = default)
        {
            var userId = GetUserId();
            var cart = await _service.AddToCartAsync(userId, dto, ct);
            return Ok(new { statusCode = 200, message = "item added", data = cart });
        }

        [HttpPut("items")]
        public async Task<ActionResult> UpdateItem([FromBody] UpdateCartItemDto dto, CancellationToken ct = default)
        {
            var userId = GetUserId();
            var cart = await _service.UpdateCartItemAsync(userId, dto, ct);
            return Ok(new { statusCode = 200, message = "item updated", data = cart });
        }

        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> RemoveItem(int cartItemId, CancellationToken ct = default)
        {
            var userId = GetUserId();
            var ok = await _service.RemoveFromCartAsync(userId, cartItemId, ct);
            if (!ok) return NotFound();
            return Ok(new { statusCode = 200, message = "item deleted", data = ok });

           
        }
        [HttpDelete]
        public async Task<IActionResult> Clear(CancellationToken ct = default)
        {
            var userId = GetUserId();
            var ok = await _service.ClearCartAsync(userId, ct);
            if (!ok) return NotFound();
            return Ok(new { statusCode = 200, message = "cart cleared", data = ok });
        }
    }
}
