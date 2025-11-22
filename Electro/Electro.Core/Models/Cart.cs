using Electro.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount => CartItems.Sum(item => item.TotalPrice);
        public AppUser User { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}
