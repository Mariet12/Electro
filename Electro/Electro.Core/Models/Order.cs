// Electro.Core.Models/Order.cs
namespace Electro.Core.Models
{
    public enum OrderStatus
    {
        Pending = 1,
        InProcessing = 2,
        Completed = 3,
        Cancelled = 4
    }

    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public bool PaymentStatus { get; set; } = false;
        public decimal TotalAmount { get; set; }

        // بيانات العميل المُضافة
        public string FullName { get; set; }          // اسم طالب الطلب
        public string PhoneNumber { get; set; }       // رقم الموبايل
        public string? Email { get; set; }            // اختياري

        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> OrderItems { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
