using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Electro.Core.Models
{
    public enum BannerScope { All = 0, Category = 1, Product = 2 }
    public enum DiscountType { Percentage = 0, FixedAmount = 1 }

    public class Banner
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;           // اسم العرض
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }                   // صورة البانر لواجهة الموقع
        public BannerScope Scope { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }              // نسبة % أو مبلغ ثابت حسب النوع
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public bool IsActive { get; set; } = true;              // للتفعيل/التعطيل السريع

        // Targets (اختياري حسب الـScope)
        public List<BannerProduct> BannerProducts { get; set; } = new();
        public List<BannerCategory> BannerCategories { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    public class BannerProduct
    {
        public int Id { get; set; }
        public int BannerId { get; set; }
        [JsonIgnore]    
        public Banner Banner { get; set; } = default!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }

    public class BannerCategory
    {
        public int Id { get; set; }
        public int BannerId { get; set; }
        [JsonIgnore]           
        public Banner Banner { get; set; } = default!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
    }

}
