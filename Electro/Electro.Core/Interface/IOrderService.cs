using Electro.Core.Dtos;
using Electro.Core.Dtos.Checkout;
using Electro.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Interface
{
    public interface IOrderService
    {
        Task<OrderDto> CheckoutAsync(string userId, CheckoutDto dto);
        Task<OrderDto?> GetOrderAsync(int orderId, string userId);
        Task<PagedResult<OrderDto>> GetAllOrdersAsync(OrderStatus? status, int pageNumber, int pageSize);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);

        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<bool> CancelOrderAsync(int orderId, string userId);

        Task<bool?> SetPaymentStatusAsync(int orderId, bool isPaid);
        Task<bool?> TogglePaymentStatusAsync(int orderId);
    }

}
