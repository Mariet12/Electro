using Electro.Core.Interface;
using Electro.Core.Models;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace Electro.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly AppIdentityDbContext _context;
        public PromotionService(AppIdentityDbContext context) => _context = context;

        public async Task<List<Banner>> GetActiveBannersAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await _context.Banners
                .Include(b => b.BannerProducts)
                .Include(b => b.BannerCategories)
                .Where(b => b.IsActive && b.StartAt <= now && b.EndAt >= now)
                .OrderByDescending(b => b.DiscountValue)
                .ToListAsync(ct);
        }

        public async Task<Banner?> GetBestBannerForAsync(int productId, int categoryId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var q = _context.Banners
                .Include(b => b.BannerProducts)
                .Include(b => b.BannerCategories)
                .Where(b => b.IsActive && b.StartAt <= now && b.EndAt >= now);

            q = q.Where(b =>
                b.Scope == BannerScope.All ||
                (b.Scope == BannerScope.Category && b.BannerCategories.Any(x => x.CategoryId == categoryId)) ||
                (b.Scope == BannerScope.Product && b.BannerProducts.Any(x => x.ProductId == productId)));

            return await q
                .OrderByDescending(b => b.DiscountValue)
                .FirstOrDefaultAsync(ct);
        }

        public decimal ApplyDiscount(decimal price, Banner banner)
        {
            if (banner.DiscountType == DiscountType.Percentage)
            {
                var pct = banner.DiscountValue / 100m;
                var discounted = price - (price * pct);
                return Math.Max(discounted, 0);
            }
            else // FixedAmount
            {
                var discounted = price - banner.DiscountValue;
                return Math.Max(discounted, 0);
            }
        }
    }
}
