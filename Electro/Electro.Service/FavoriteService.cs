using Electro.Core.Dtos;
using Electro.Core.Dtos.Favorite;
using Electro.Core.Dtos.Product;
using Electro.Core.Interface;
using Electro.Core.Models;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Service
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppIdentityDbContext _context;

        public FavoriteService(AppIdentityDbContext context) => _context = context;

        public async Task<PagedResult<FavoriteDto>> GetUserFavoritesAsync(string userId, PaginationParams paging, CancellationToken ct = default)
        {
            var q = _context.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .Include(f => f.Product).ThenInclude(p => p.Category)
                .Include(f => f.Product).ThenInclude(p => p.ProductImages)
                .OrderByDescending(f => f.AddedAt);

            var total = await q.CountAsync(ct);

            var items = await q
                .Skip((paging.PageNumber - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .Select(f => new FavoriteDto
                {
                    Id = f.Id,
                    AddedAt = f.AddedAt,
                    Product = new ProductDto
                    {
                        Id = f.Product.Id,
                        Name_Ar = f.Product.Name_Ar,
                        Name_En = f.Product.Name_En,
                        Description = f.Product.Description,
                        CountryOfOrigin = f.Product.CountryOfOrigin,
                        Brand = f.Product.Brand,
                        Warranty = f.Product.Warranty,
                        Price = f.Product.Price,
                        CategoryId = f.Product.CategoryId,
                        CategoryName = f.Product.Category != null ? f.Product.Category.Name : null,
                        FirstImageUrl = f.Product.ProductImages.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                        Images = f.Product.ProductImages.Select(i => new ProductImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            ProductId = i.ProductId
                        }).ToList(),
                        IsFavorite = true,
                        IsInCart = _context.CartItems.Any(ci => ci.ProductId == f.ProductId && ci.Cart.UserId == userId)
                    }
                })
                .ToListAsync(ct);

            return new PagedResult<FavoriteDto> { PageNumber = paging.PageNumber, PageSize = paging.PageSize, TotalCount = total, Items = items };
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int productId, CancellationToken ct = default)
        {
            var exists = await _context.Products.AnyAsync(p => p.Id == productId && !p.IsDeleted, ct);
            if (!exists) return false;

            var already = await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId, ct);
            if (already) return true;

            _context.Favorites.Add(new Favorite { UserId = userId, ProductId = productId });
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int productId, CancellationToken ct = default)
        {
            var fav = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId, ct);
            if (fav == null) return false;
            _context.Favorites.Remove(fav);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> IsInFavoritesAsync(string userId, int productId, CancellationToken ct = default)
            => _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId, ct);
    }

}
