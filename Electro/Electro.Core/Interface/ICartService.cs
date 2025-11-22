using Electro.Core.Dtos.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Interface
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string userId, CancellationToken ct = default);
        Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto, CancellationToken ct = default);
        Task<CartDto> UpdateCartItemAsync(string userId, UpdateCartItemDto dto, CancellationToken ct = default);
        Task<bool> RemoveFromCartAsync(string userId, int cartItemId, CancellationToken ct = default);
        Task<bool> ClearCartAsync(string userId, CancellationToken ct = default);
    }
}
