using Electro.Core.Dtos.Cart;
using Electro.Core.Interface;
using Electro.Core.Models;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace Electro.Service
{
    public class CartService : ICartService
    {
        private readonly AppIdentityDbContext _context;
        private readonly IPromotionService _promos;

        public CartService(AppIdentityDbContext context, IPromotionService promotion)
        {
            _context = context;
            _promos = promotion;
        }

        public async Task<CartDto> GetCartAsync(string userId, CancellationToken ct = default)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product).ThenInclude(p => p.ProductImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(ct);

                cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .AsNoTracking()
                    .FirstAsync(c => c.Id == cart.Id, ct);
            }

            // لو عايز تسعير ديناميكي عند القراءة، فكّ الكومنت السطرين دول:
            // cart = await RepriceCartAsync(cart.Id, ct);
            // return MapToCartDto(cart);

            return MapToCartDto(cart);
        }

        public async Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto, CancellationToken ct = default)
        {
            var cart = await GetOrCreateCartAsync(userId, ct);

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId && !p.IsDeleted, ct)
                ?? throw new ArgumentException("Product not found");

            var price = (decimal)product.Price;
            var banner = await _promos.GetBestBannerForAsync(product.Id, product.CategoryId, ct);
            var unitPrice = banner != null ? _promos.ApplyDiscount(price, banner) : price;

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (item != null)
            {
                item.Quantity += dto.Quantity;
                // اختياري: ثبّت آخر سعر مطبّق
                item.UnitPrice = unitPrice;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = unitPrice
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            // رجّع السلة محدثة (قراءة بدون تتبّع)
            var fresh = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.ProductImages)
                .AsNoTracking()
                .FirstAsync(c => c.Id == cart.Id, ct);

            return MapToCartDto(fresh);
        }

        public async Task<CartDto> UpdateCartItemAsync(string userId, UpdateCartItemDto dto, CancellationToken ct = default)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct)
                ?? throw new ArgumentException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == dto.CartItemId)
                ?? throw new ArgumentException("Cart item not found");

            if (dto.Quantity <= 0)
            {
                cart.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = dto.Quantity;

                // لو عايز السعر يتحدّث عند التعديل، أعد حساب الخصم هنا (اختياري):
                // var product = await _context.Products.Include(p => p.Category).FirstAsync(p => p.Id == item.ProductId, ct);
                // var price = (decimal)product.Price;
                // var banner = await _promos.GetBestBannerForAsync(product.Id, product.CategoryId, ct);
                // item.UnitPrice = banner != null ? _promos.ApplyDiscount(price, banner) : price;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            var fresh = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.ProductImages)
                .AsNoTracking()
                .FirstAsync(c => c.Id == cart.Id, ct);

            return MapToCartDto(fresh);
        }

        public async Task<bool> RemoveFromCartAsync(string userId, int cartItemId, CancellationToken ct = default)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart == null) return false;

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null) return false;

            cart.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ClearCartAsync(string userId, CancellationToken ct = default)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart == null) return false;

            cart.CartItems.Clear();
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// إعادة تسعير السلة بالكامل بالاعتماد على البانرات الحالية (اختياري).
        /// </summary>
        public async Task<Cart> RepriceCartAsync(int cartId, CancellationToken ct = default)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product).ThenInclude(p => p.Category)
                .FirstAsync(c => c.Id == cartId, ct);

            foreach (var ci in cart.CartItems)
            {
                if (ci.Product == null || ci.Product.IsDeleted) continue;

                var price = (decimal)ci.Product.Price;
                var banner = await _promos.GetBestBannerForAsync(ci.ProductId, ci.Product.CategoryId, ct);
                var unitPrice = banner != null ? _promos.ApplyDiscount(price, banner) : price;

                ci.UnitPrice = unitPrice;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            // أعد القراءة بدون تتبع للعرض
            return await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.ProductImages)
                .AsNoTracking()
                .FirstAsync(c => c.Id == cart.Id, ct);
        }

        private async Task<Cart> GetOrCreateCartAsync(string userId, CancellationToken ct)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart != null) return cart;

            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(ct);

            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstAsync(c => c.Id == cart.Id, ct);
        }

        private static CartDto MapToCartDto(Cart cart)
        {
            var items = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product?.Name_En ?? ci.Product?.Name_Ar,
                ProductImage = ci.Product?.ProductImages?.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                TotalPrice = ci.TotalPrice,
                AddedAt = ci.AddedAt
            }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = items,
                ItemsCount = items.Sum(i => i.Quantity),
                TotalAmount = items.Sum(i => i.TotalPrice)
            };
        }
    }
}
