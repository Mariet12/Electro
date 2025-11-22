using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name_Ar { get; set; }
        public string Name_En { get; set; }
        public string Description { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Brand { get; set; }
        public string Warranty { get; set; }
        public double Price {  get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft delete flag

    }
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
