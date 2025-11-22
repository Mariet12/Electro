using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }

}
