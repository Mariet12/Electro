using Electro.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Electro.Core.Dtos.Checkout
{
    public class CheckoutDto
    {
        [Required, StringLength(150)]
        public string FullName { get; set; }

        [Required, StringLength(20)]
        public string PhoneNumber { get; set; }

        [EmailAddress, StringLength(150)]
        public string? Email { get; set; }

        // ممكن تسيبها كسطر واحد أو تفصلها لاحقاً (City/Area/...)
        [Required, StringLength(500)]
        public string ShippingAddress { get; set; }

        [Required, StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusText { get; set; }
        public decimal TotalAmount { get; set; }
        public bool PaymentStatus { get; set; } = false;

        // بيانات العميل
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }

        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
