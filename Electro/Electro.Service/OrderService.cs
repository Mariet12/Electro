using Electro.Core.Dtos;
using Electro.Core.Dtos.Checkout;
using Electro.Core.Interface;
using Electro.Core.Models;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace Electro.Service
{
    public class OrderService : IOrderService
    {
        private readonly AppIdentityDbContext _context;
        private readonly ICartService _cartService;
        private readonly IPromotionService _promos;
        private readonly INotificationService _notify; // ✨ جديد

        public OrderService(
            AppIdentityDbContext context,
            ICartService cartService,
            IPromotionService promos,
            INotificationService notify) // ✨ جديد
        {
            _context = context;
            _cartService = cartService;
            _promos = promos;
            _notify = notify; // ✨ جديد
        }

        public async Task<OrderDto> CheckoutAsync(string userId, CheckoutDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.ShippingAddress))
                throw new InvalidOperationException("يرجى إدخال الاسم ورقم الجوال والعنوان.");

            // إعادة تسعير لضمان الخصومات لحظة الدفع
            foreach (var ci in cart.CartItems)
            {
                if (ci.Product == null || ci.Product.IsDeleted) continue;

                if (ci.Product.CategoryId == 0)
                {
                    var prod = await _context.Products
                        .Include(p => p.Category)
                        .FirstAsync(p => p.Id == ci.ProductId);
                    ci.Product = prod;
                }

                var basePrice = (decimal)ci.Product.Price;
                var banner = await _promos.GetBestBannerForAsync(ci.ProductId, ci.Product.CategoryId);
                ci.UnitPrice = banner != null ? _promos.ApplyDiscount(basePrice, banner) : basePrice;
            }

            await _context.SaveChangesAsync();

            var totalAmount = cart.CartItems.Sum(i => i.TotalPrice);

            var order = new Order
            {
                UserId = userId,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount,

                FullName = dto.FullName.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = false,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    TotalPrice = ci.TotalPrice
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 🔔 إشعار للمستخدم: تم إنشاء الطلب
            await _notify.SendAndStoreAsync(
                receiverId: userId,
                titleAr: "تم إنشاء طلبك",
                bodyAr: $"رقم الطلب: {order.OrderNumber} — الإجمالي {order.TotalAmount:N2}",
                status: "OrderCreated",
                orderId: order.Id,
                data: new() { ["type"] = "order", ["action"] = "created" }
            );

            // 🔔 إشعار للمديرين (اختياري)
            await _notify.SendNotificationToAdmins(
                senderId: userId,
                title: "طلب جديد",
                message: $"تم إنشاء طلب جديد رقم {order.OrderNumber}",
                status: "OrderCreated",
                orderId: order.Id
            );

            // امسح السلة بعد إنشاء الطلب
            await _cartService.ClearCartAsync(userId);

            var created = await GetOrderAsync(order.Id, userId);
            return created!;
        }

        public async Task<OrderDto?> GetOrderAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product).ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            return order == null ? null : MapToOrderDto(order);
        }

        public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(OrderStatus? status, int pageNumber, int pageSize)
        {
            var q = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.ProductImages)
                .AsNoTracking()
                .AsQueryable();

            if (status.HasValue) q = q.Where(o => o.Status == status.Value);

            var total = await q.CountAsync();
            var items = await q
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => MapToOrderDto(o))
                .ToListAsync();

            return new PagedResult<OrderDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.ProductImages)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return orders.Select(MapToOrderDto);
        }

        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return null;

            bool allowed = order.Status switch
            {
                OrderStatus.Pending => newStatus is OrderStatus.InProcessing or OrderStatus.Cancelled,
                OrderStatus.InProcessing => newStatus is OrderStatus.Completed or OrderStatus.Cancelled,
                _ => false
            };

            if (!allowed)
                throw new InvalidOperationException($"لا يمكن التحويل من {order.Status} إلى {newStatus}");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 🔔 إشعار للمستخدم بتغيير الحالة
            var statusText = GetStatusText(newStatus);
            await _notify.SendAndStoreAsync(
                receiverId: order.UserId,
                titleAr: "تحديث حالة الطلب",
                bodyAr: $"تم تحديث حالة طلبك ({order.OrderNumber}) إلى: {statusText}",
                status: "OrderStatusChanged",
                orderId: order.Id,
                data: new() { ["type"] = "order", ["action"] = "status", ["status"] = newStatus.ToString() }
            );

            var dto = await GetOrderAsync(order.Id, order.UserId);
            return dto;
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (order == null) return false;

            if (order.Status != OrderStatus.Pending)
                return false;

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 🔔 إشعار إلغاء
            await _notify.SendAndStoreAsync(
                receiverId: userId,
                titleAr: "تم إلغاء الطلب",
                bodyAr: $"تم إلغاء طلبك ({order.OrderNumber}).",
                status: "OrderCancelled",
                orderId: order.Id,
                data: new() { ["type"] = "order", ["action"] = "cancelled" }
            );

            return true;
        }

        public async Task<bool?> SetPaymentStatusAsync(int orderId, bool isPaid)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return null;

            order.PaymentStatus = isPaid;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 🔔 إشعار دفع
            await _notify.SendAndStoreAsync(
                receiverId: order.UserId,
                titleAr: isPaid ? "تم تأكيد الدفع" : "تم إلغاء الدفع",
                bodyAr: isPaid
                    ? $"تم تأكيد دفع الطلب ({order.OrderNumber}). شكراً لك!"
                    : $"تم تغيير حالة دفع الطلب ({order.OrderNumber}) إلى غير مدفوع.",
                status: "PaymentUpdated",
                orderId: order.Id,
                data: new() { ["type"] = "order", ["action"] = "payment", ["paid"] = isPaid.ToString() }
            );

            return order.PaymentStatus;
        }

        public async Task<bool?> TogglePaymentStatusAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return null;

            order.PaymentStatus = !order.PaymentStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 🔔 إشعار دفع
            await _notify.SendAndStoreAsync(
                receiverId: order.UserId,
                titleAr: order.PaymentStatus ? "تم تأكيد الدفع" : "تم إلغاء الدفع",
                bodyAr: order.PaymentStatus
                    ? $"تم تأكيد دفع الطلب ({order.OrderNumber})."
                    : $"تم تغيير حالة دفع الطلب ({order.OrderNumber}) إلى غير مدفوع.",
                status: "PaymentUpdated",
                orderId: order.Id,
                data: new() { ["type"] = "order", ["action"] = "payment", ["paid"] = order.PaymentStatus.ToString() }
            );

            return order.PaymentStatus;
        }

        private static string GenerateOrderNumber()
            => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

        private static OrderDto MapToOrderDto(Order order)
            => new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                StatusText = GetStatusText(order.Status),
                TotalAmount = order.TotalAmount,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                Email = order.Email,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name_En ?? oi.Product?.Name_Ar,
                    ProductImage = oi.Product?.ProductImages?.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };

        private static string GetStatusText(OrderStatus status) => status switch
        {
            OrderStatus.Pending => "في الانتظار",
            OrderStatus.InProcessing => "قيد التنفيذ",
            OrderStatus.Completed => "مكتمل",
            OrderStatus.Cancelled => "ملغي",
            _ => "غير معروف"
        };
    }
}
