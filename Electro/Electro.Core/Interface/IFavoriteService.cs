using Electro.Core.Dtos;
using Electro.Core.Dtos.Favorite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Interface
{
    public interface IFavoriteService
    {
        Task<PagedResult<FavoriteDto>> GetUserFavoritesAsync(string userId, PaginationParams paging, CancellationToken ct = default);
        Task<bool> AddToFavoritesAsync(string userId, int productId, CancellationToken ct = default);
        Task<bool> RemoveFromFavoritesAsync(string userId, int productId, CancellationToken ct = default);
        Task<bool> IsInFavoritesAsync(string userId, int productId, CancellationToken ct = default);
    }

}
