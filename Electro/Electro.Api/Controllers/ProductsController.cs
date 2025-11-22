using Electro.Core.Dtos;
using Electro.Core.Dtos.Product;
using Electro.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Electro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service) => _service = service;

        // طلب لست المنتجات مع كل الفلاتر والـ pagination
    [HttpGet]
    public async Task<ActionResult> GetProducts(
    [FromQuery] int? categoryId,
    [FromQuery] string? search,
    [FromQuery] string? brand,
    [FromQuery] double? priceFrom,
    [FromQuery] double? priceTo,
    [FromQuery] string? countryOfOrigin,
    [FromQuery] string? sortBy = "new",
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken ct = default)
    {
        // نحاول نقرأ userId من التوكن لو موجود
        string? userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        var query = new ProductQuery
        {
            UserId = userId,
            CategoryId = categoryId,
            Search = search,
            Brand = brand,
            PriceFrom = priceFrom,
            PriceTo = priceTo,
            CountryOfOrigin = countryOfOrigin,
            SortBy = sortBy,
            Paging = new PaginationParams { PageNumber = pageNumber, PageSize = pageSize }
        };

        var result = await _service.GetAsync(query, ct);
        return Ok(new { statusCode = 200, message = "all data", data = result });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetById(int id, CancellationToken ct = default)
    {
        string? userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        var dto = await _service.GetByIdAsync(id, userId, ct);
        if (dto is null) return NotFound();
        return Ok(new { statusCode = 200, message = "product", data = dto });
    }


    // Create (multipart/form-data) — صور في dto.Images
    [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromForm] CreateProductDto dto, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(dto, ct);
            return Ok(new { statusCode = 200, message = "product added", data = created });
        }

        // Update (multipart/form-data) لدعم إضافة/حذف الصور
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromForm] UpdateProductDto dto, CancellationToken ct = default)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            if (updated is null) return NotFound();
            return Ok(new { statusCode = 200, message = "product updated", data = updated });
        }

        // Soft Delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok) return NotFound();
            return Ok(new { statusCode = 200, message = "product deleted", data = ok });
        }
        [HttpGet("latest")]
        public async Task<ActionResult> GetLatest([FromQuery] int take = 10, CancellationToken ct = default)
        {
            string? userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            var data = await _service.GetLatestAsync(userId, take, ct);
            return Ok(new { statusCode = 200, message = "latest products", data });
        }

        [HttpGet("best-selling")]
        public async Task<ActionResult> GetBestSelling(
            [FromQuery] int take = 10,
            [FromQuery] int? days = null,
            CancellationToken ct = default)
        {
            string? userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            var data = await _service.GetBestSellingAsync(userId, take, days, ct);
            return Ok(new { statusCode = 200, message = "best-selling products", data });
        }

    }
}
