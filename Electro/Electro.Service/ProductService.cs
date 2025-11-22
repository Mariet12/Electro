using Electro.Core.Dtos.Product;
using Electro.Core.Dtos;
using Electro.Core.Interface;
using Electro.Core.Models;
using Electro.Reposatory.Data.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

public class ProductService : IProductService
{
    private readonly AppIdentityDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly string _baseUrl;
    private readonly IPromotionService _promos;

    public ProductService(
        AppIdentityDbContext context,
        IWebHostEnvironment environment,
        IConfiguration config,
        IPromotionService promos)
    {
        _context = context;
        _environment = environment;
        _baseUrl = config.GetValue<string>("BaseUrl") ?? "https://localhost:5001";
        _promos = promos;
    }

    // واحد موحّد للفلترة والبحث والـ pagination
    public async Task<PagedResult<ProductDto>> GetAsync(ProductQuery query, CancellationToken ct = default)
    {
        var q = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (query.CategoryId.HasValue)
            q = q.Where(p => p.CategoryId == query.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(query.Brand))
            q = q.Where(p => p.Brand!.Contains(query.Brand));

        if (!string.IsNullOrWhiteSpace(query.CountryOfOrigin))
            q = q.Where(p => p.CountryOfOrigin!.Contains(query.CountryOfOrigin));

        if (query.PriceFrom.HasValue)
            q = q.Where(p => p.Price >= query.PriceFrom.Value);

        if (query.PriceTo.HasValue)
            q = q.Where(p => p.Price <= query.PriceTo.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            q = q.Where(p =>
                (p.Name_Ar != null && p.Name_Ar.Contains(s)) ||
                (p.Name_En != null && p.Name_En.Contains(s)) ||
                (p.Description != null && p.Description.Contains(s)) ||
                (p.Brand != null && p.Brand.Contains(s)));
        }

        // Sorting
        q = query.SortBy?.ToLowerInvariant() switch
        {
            "price" => q.OrderBy(p => p.Price).ThenByDescending(p => p.Id),
            "-price" => q.OrderByDescending(p => p.Price).ThenByDescending(p => p.Id),
            "name" => q.OrderBy(p => (p.Name_En ?? p.Name_Ar)).ThenByDescending(p => p.Id),
            "-name" => q.OrderByDescending(p => (p.Name_En ?? p.Name_Ar)).ThenByDescending(p => p.Id),
            _ => q.OrderByDescending(p => p.Id) // new
        };

        var total = await q.CountAsync(ct);

        var items = await q
            .Skip((query.Paging.PageNumber - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name_Ar = p.Name_Ar,
                Name_En = p.Name_En,
                Description = p.Description,
                CountryOfOrigin = p.CountryOfOrigin,
                Brand = p.Brand,
                Warranty = p.Warranty,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                FirstImageUrl = p.ProductImages.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                Images = p.ProductImages.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    ProductId = img.ProductId
                }).ToList(),
                // Flags (EXISTS subqueries)
                IsFavorite = !string.IsNullOrEmpty(query.UserId) &&
                             _context.Favorites.Any(f => f.UserId == query.UserId && f.ProductId == p.Id),
                IsInCart = !string.IsNullOrEmpty(query.UserId) &&
                             _context.CartItems.Any(ci => ci.ProductId == p.Id && ci.Cart.UserId == query.UserId),

                // الحقول الجديدة (هيتم ملأها بعدين بالذاكرة):
                EffectivePrice = 0,        // placeholder
                HasDiscount = false,
                AppliedBannerId = null,
                AppliedBannerTitle = null,
                DiscountType = null,
                DiscountValue = null
            })
            .ToListAsync(ct);

        // === تطبيق الخصومات بالذاكرة لتقليل الاستعلامات ===
        var activeBanners = await _promos.GetActiveBannersAsync(ct); // فعّالة فقط

        foreach (var p in items)
        {
            var price = (decimal)p.Price;
            var banner = ChooseBestBanner(activeBanners, p.Id, p.CategoryId);
            if (banner != null)
            {
                var eff = _promos.ApplyDiscount(price, banner);
                p.EffectivePrice = eff;
                p.HasDiscount = eff < price;
                p.AppliedBannerId = banner.Id;
                p.AppliedBannerTitle = banner.Title;
                p.DiscountType = banner.DiscountType;
                p.DiscountValue = banner.DiscountValue;
            }
            else
            {
                p.EffectivePrice = price;
                p.HasDiscount = false;
            }
        }

        return new PagedResult<ProductDto>
        {
            PageNumber = query.Paging.PageNumber,
            PageSize = query.Paging.PageSize,
            TotalCount = total,
            Items = items
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id, string? userId = null, CancellationToken ct = default)
    {
        var dto = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Where(p => !p.IsDeleted && p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name_Ar = p.Name_Ar,
                Name_En = p.Name_En,
                Description = p.Description,
                CountryOfOrigin = p.CountryOfOrigin,
                Brand = p.Brand,
                Warranty = p.Warranty,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                FirstImageUrl = p.ProductImages.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                Images = p.ProductImages.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    ProductId = img.ProductId
                }).ToList(),
                IsFavorite = userId != null &&
                             _context.Favorites.Any(f => f.UserId == userId && f.ProductId == p.Id),
                IsInCart = userId != null &&
                             _context.CartItems.Any(ci => ci.ProductId == p.Id && ci.Cart.UserId == userId),

                // placeholders سيتم ضبطها بعدين
                EffectivePrice = 0,
                HasDiscount = false,
                AppliedBannerId = null,
                AppliedBannerTitle = null,
                DiscountType = null,
                DiscountValue = null
            })
            .FirstOrDefaultAsync(ct);

        if (dto == null) return null;

        var activeBanners = await _promos.GetActiveBannersAsync(ct);
        var banner = ChooseBestBanner(activeBanners, dto.Id, dto.CategoryId);
        var price = (decimal)dto.Price;

        if (banner != null)
        {
            var eff = _promos.ApplyDiscount(price, banner);
            dto.EffectivePrice = eff;
            dto.HasDiscount = eff < price;
            dto.AppliedBannerId = banner.Id;
            dto.AppliedBannerTitle = banner.Title;
            dto.DiscountType = banner.DiscountType;
            dto.DiscountValue = banner.DiscountValue;
        }
        else
        {
            dto.EffectivePrice = price;
            dto.HasDiscount = false;
        }

        return dto;
    }

    // Create / Update / Delete
    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId, ct);
        if (!categoryExists) throw new ArgumentException("Category not found");

        var product = new Product
        {
            Name_Ar = dto.Name_Ar,
            Name_En = dto.Name_En,
            Description = dto.Description,
            CountryOfOrigin = dto.CountryOfOrigin,
            Brand = dto.Brand,
            Warranty = dto.Warranty,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            ProductImages = new List<ProductImage>()
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);

        if (dto.Images?.Any() == true)
        {
            var imageUrls = await SaveImagesAsync(dto.Images, ct);
            foreach (var url in imageUrls)
                product.ProductImages.Add(new ProductImage { ImageUrl = url, ProductId = product.Id });

            await _context.SaveChangesAsync(ct);
        }

        var created = await GetByIdAsync(product.Id, null, ct);
        return created!;
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

        if (product == null) return null;

        if (!string.IsNullOrEmpty(dto.Name_Ar)) product.Name_Ar = dto.Name_Ar;
        if (!string.IsNullOrEmpty(dto.Name_En)) product.Name_En = dto.Name_En;
        if (!string.IsNullOrEmpty(dto.Description)) product.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.CountryOfOrigin)) product.CountryOfOrigin = dto.CountryOfOrigin;
        if (!string.IsNullOrEmpty(dto.Brand)) product.Brand = dto.Brand;
        if (!string.IsNullOrEmpty(dto.Warranty)) product.Warranty = dto.Warranty;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;

        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value, ct);
            if (!categoryExists) throw new ArgumentException("Category not found");
            product.CategoryId = dto.CategoryId.Value;
        }

        if (dto.ImageIdsToDelete?.Any() == true)
        {
            var imagesToDelete = product.ProductImages.Where(img => dto.ImageIdsToDelete.Contains(img.Id)).ToList();
            foreach (var image in imagesToDelete)
            {
                DeleteImageFile(image.ImageUrl);
                _context.ProductImages.Remove(image);
            }
        }

        if (dto.NewImages?.Any() == true)
        {
            var newUrls = await SaveImagesAsync(dto.NewImages, ct);
            foreach (var url in newUrls)
                product.ProductImages.Add(new ProductImage { ImageUrl = url, ProductId = product.Id });
        }

        await _context.SaveChangesAsync(ct);
        return await GetByIdAsync(product.Id, null, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var product = await _context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);
        if (product == null) return false;

        foreach (var image in product.ProductImages)
            DeleteImageFile(image.ImageUrl);

        product.IsDeleted = true;
        await _context.SaveChangesAsync(ct);
        return true;
    }
    public async Task<IReadOnlyList<ProductDto>> GetLatestAsync(string? userId, int take = 10, CancellationToken ct = default)
    {
        take = take <= 0 ? 10 : take;

        var items = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Where(p => !p.IsDeleted)
            //.OrderByDescending(p => p.CreatedDate)
            .OrderByDescending(p => p.Id) // fallback لو مفيش CreatedDate
            .Take(take)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name_Ar = p.Name_Ar,
                Name_En = p.Name_En,
                Description = p.Description,
                CountryOfOrigin = p.CountryOfOrigin,
                Brand = p.Brand,
                Warranty = p.Warranty,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                FirstImageUrl = p.ProductImages.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                Images = p.ProductImages.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    ProductId = img.ProductId
                }).ToList(),
                IsFavorite = !string.IsNullOrEmpty(userId) &&
                             _context.Favorites.Any(f => f.UserId == userId && f.ProductId == p.Id),
                IsInCart = !string.IsNullOrEmpty(userId) &&
                           _context.CartItems.Any(ci => ci.ProductId == p.Id && ci.Cart.UserId == userId),
                EffectivePrice = 0,
                HasDiscount = false,
                AppliedBannerId = null,
                AppliedBannerTitle = null,
                DiscountType = null,
                DiscountValue = null
            })
            .ToListAsync(ct);

        var activeBanners = await _promos.GetActiveBannersAsync(ct);
        foreach (var p in items)
        {
            var price = (decimal)p.Price;
            var banner = ChooseBestBanner(activeBanners, p.Id, p.CategoryId);
            if (banner != null)
            {
                var eff = _promos.ApplyDiscount(price, banner);
                p.EffectivePrice = eff;
                p.HasDiscount = eff < price;
                p.AppliedBannerId = banner.Id;
                p.AppliedBannerTitle = banner.Title;
                p.DiscountType = banner.DiscountType;
                p.DiscountValue = banner.DiscountValue;
            }
            else { p.EffectivePrice = price; p.HasDiscount = false; }
        }

        return items;
    }
    public async Task<IReadOnlyList<ProductDto>> GetBestSellingAsync(
        string? userId, int take = 10, int? days = null, CancellationToken ct = default)
    {
        take = take <= 0 ? 10 : take;

        var orderItems = _context.OrderItems
            .AsNoTracking()
            .Where(oi => oi.Order != null
                         && oi.Order.Status == OrderStatus.Completed
                         && oi.Order.PaymentStatus);

        if (days.HasValue && days.Value > 0)
        {
            var from = DateTime.UtcNow.AddDays(-days.Value);
            orderItems = orderItems.Where(oi => oi.Order!.CreatedAt >= from);
        }

        var top = await orderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g => new { ProductId = g.Key, Qty = g.Sum(x => x.Quantity) })
            .OrderByDescending(x => x.Qty)
            .Take(take)
            .ToListAsync(ct);

        if (top.Count == 0) return new List<ProductDto>();

        var topIds = top.Select(t => t.ProductId).ToList();
        var rank = top.Select((t, i) => new { t.ProductId, Index = i })
                      .ToDictionary(x => x.ProductId, x => x.Index);

        var items = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Where(p => !p.IsDeleted && topIds.Contains(p.Id))
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name_Ar = p.Name_Ar,
                Name_En = p.Name_En,
                Description = p.Description,
                CountryOfOrigin = p.CountryOfOrigin,
                Brand = p.Brand,
                Warranty = p.Warranty,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                FirstImageUrl = p.ProductImages.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                Images = p.ProductImages.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    ProductId = img.ProductId
                }).ToList(),
                IsFavorite = !string.IsNullOrEmpty(userId) &&
                             _context.Favorites.Any(f => f.UserId == userId && f.ProductId == p.Id),
                IsInCart = !string.IsNullOrEmpty(userId) &&
                           _context.CartItems.Any(ci => ci.ProductId == p.Id && ci.Cart.UserId == userId),
                EffectivePrice = 0,
                HasDiscount = false,
                AppliedBannerId = null,
                AppliedBannerTitle = null,
                DiscountType = null,
                DiscountValue = null
            })
            .ToListAsync(ct);

        // ترتيب حسب أعلى مبيعات
        items = items.OrderBy(p => rank[p.Id]).ToList();

        // تطبيق الخصومات
        var activeBanners = await _promos.GetActiveBannersAsync(ct);
        foreach (var p in items)
        {
            var price = (decimal)p.Price;
            var banner = ChooseBestBanner(activeBanners, p.Id, p.CategoryId);
            if (banner != null)
            {
                var eff = _promos.ApplyDiscount(price, banner);
                p.EffectivePrice = eff;
                p.HasDiscount = eff < price;
                p.AppliedBannerId = banner.Id;
                p.AppliedBannerTitle = banner.Title;
                p.DiscountType = banner.DiscountType;
                p.DiscountValue = banner.DiscountValue;
            }
            else { p.EffectivePrice = price; p.HasDiscount = false; }
        }

        return items;
    }


    // Helpers
    private async Task<List<string>> SaveImagesAsync(List<IFormFile> images, CancellationToken ct)
    {
        var urls = new List<string>();
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        foreach (var image in images.Where(i => i.Length > 0))
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream, ct);
            urls.Add($"{_baseUrl}/uploads/products/{fileName}");
        }
        return urls;
    }

    private void DeleteImageFile(string imageUrl)
    {
        try
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "products", fileName);
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        catch { /* log */ }
    }

    // اختيار أفضل بانر بالذاكرة (بدون Priority): أكبر DiscountValue ينطبق
    private static Banner? ChooseBestBanner(IEnumerable<Banner> activeBanners, int productId, int categoryId)
    {
        return activeBanners
            .Where(b =>
                b.Scope == BannerScope.All ||
                (b.Scope == BannerScope.Category && b.BannerCategories.Any(x => x.CategoryId == categoryId)) ||
                (b.Scope == BannerScope.Product && b.BannerProducts.Any(x => x.ProductId == productId)))
            .OrderByDescending(b => b.DiscountValue)
            .FirstOrDefault();
    }
}
