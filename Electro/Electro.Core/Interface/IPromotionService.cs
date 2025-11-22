using Electro.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Interface
{
    public interface IPromotionService
    {
        Task<Banner?> GetBestBannerForAsync(int productId, int categoryId, CancellationToken ct = default);
        decimal ApplyDiscount(decimal price, Banner banner);
        Task<List<Banner>> GetActiveBannersAsync(CancellationToken ct = default);
    }
}
