using Electro.Core.Models;
using Electro.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Electro.Reposatory.Data.Identity;
using Electro.Core.Dtos.Banner;

namespace Electro.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/banners")]
    public class BannersController : ControllerBase
    {
        private readonly AppIdentityDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BannersController(AppIdentityDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // إضافة بانر جديد
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateBannerDto dto, CancellationToken ct)
        {
            if (dto.EndAt <= dto.StartAt)
                return BadRequest("تاريخ النهاية يجب أن يكون بعد البداية");

            var banner = new Banner
            {
                Title = dto.Title,
                Description = dto.Description,
                Scope = dto.Scope,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                IsActive = true
            };

            // رفع الصورة لو موجودة
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "banners");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.Image.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(fs, ct);
                }
                banner.ImageUrl = $"/uploads/banners/{fileName}";
            }

            // ربط المنتجات / التصنيفات
            if (dto.ProductIds?.Any() == true)
                banner.BannerProducts = dto.ProductIds.Select(pid => new BannerProduct { ProductId = pid }).ToList();

            if (dto.CategoryIds?.Any() == true)
                banner.BannerCategories = dto.CategoryIds.Select(cid => new BannerCategory { CategoryId = cid }).ToList();

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync(ct);
            return Ok(new { statusCode = 200, message = "data", data = banner });
        }

        // تعديل بانر
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CreateBannerDto dto, CancellationToken ct)
        {
            var banner = await _context.Banners
                .Include(b => b.BannerProducts)
                .Include(b => b.BannerCategories)
                .FirstOrDefaultAsync(b => b.Id == id, ct);

            if (banner == null) return NotFound();

            banner.Title = dto.Title;
            banner.Description = dto.Description;
            banner.Scope = dto.Scope;
            banner.DiscountType = dto.DiscountType;
            banner.DiscountValue = dto.DiscountValue;
            banner.StartAt = dto.StartAt;
            banner.EndAt = dto.EndAt;

            // Replace الصورة لو اتبعت جديدة
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "banners");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.Image.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(fs, ct);
                }
                banner.ImageUrl = $"/uploads/banners/{fileName}";
            }

            // Update العلاقات
            banner.BannerProducts.Clear();
            banner.BannerCategories.Clear();

            if (dto.ProductIds?.Any() == true)
                banner.BannerProducts = dto.ProductIds.Select(pid => new BannerProduct { ProductId = pid }).ToList();

            if (dto.CategoryIds?.Any() == true)
                banner.BannerCategories = dto.CategoryIds.Select(cid => new BannerCategory { CategoryId = cid }).ToList();

            await _context.SaveChangesAsync(ct);
            return Ok(new { statusCode = 200, message = "updated banner", });
        }

        // تعطيل / تفعيل بانر
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> Toggle(int id, CancellationToken ct)
        {
            var banner = await _context.Banners.FindAsync(new object[] { id }, ct);
            if (banner == null) return NotFound();

            banner.IsActive = !banner.IsActive;
            await _context.SaveChangesAsync(ct);
            return Ok(new { statusCode = 200, message = "data", data = banner.IsActive });
        }

        // حذف بانر
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var banner = await _context.Banners.FindAsync(new object[] { id }, ct);
            if (banner == null) return NotFound();

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync(ct);
            return Ok(new { statusCode = 200, message = "deleted" });
        }
        [HttpGet("active")]
        public async Task<IActionResult> GetActive(CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var banners = await _context.Banners
                .AsNoTracking()
                .Where(b => b.IsActive && b.StartAt <= now && b.EndAt >= now)
                .OrderByDescending(b => b.DiscountValue)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.ImageUrl,
                    b.DiscountType,
                    b.DiscountValue,
                    b.Scope
                })
                .ToListAsync(ct);

            return Ok(new {statusCode = 200,message = "all banners",data = banners });
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var banners = await _context.Banners
                .AsNoTracking()
                .OrderByDescending(b => b.DiscountValue)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.ImageUrl,
                    b.DiscountType,
                    b.DiscountValue,
                    b.Scope
                })
                .ToListAsync(ct);

            return Ok(new { statusCode = 200, message = "all banners", data = banners });
        }
        // GET: /api/admin/banners/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var banner = await _context.Banners
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.ImageUrl,
                    b.Scope,
                    b.DiscountType,
                    b.DiscountValue,
                    b.StartAt,
                    b.EndAt,
                    b.IsActive,
                    b.CreatedAt,
                    ProductIds = b.BannerProducts.Select(x => x.ProductId).ToList(),
                    CategoryIds = b.BannerCategories.Select(x => x.CategoryId).ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (banner is null)
                return NotFound(new { statusCode = 404, message = $"Banner {id} not found" });

            return Ok(new { statusCode = 200, message = "banner", data = banner });
        }

    }
}
