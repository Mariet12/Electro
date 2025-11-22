using Electro.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Product
{
    public class CreateProductDto
    {
        [Required]
        public string Name_Ar { get; set; }

        [Required]
        public string Name_En { get; set; }

        public string Description { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Brand { get; set; }
        public string Warranty { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }


        [Required]
        public int CategoryId { get; set; }

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }

    public class UpdateProductDto
    {
        public string Name_Ar { get; set; }
        public string Name_En { get; set; }
        public string Description { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Brand { get; set; }
        public string? Warranty { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double? Price { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public double? Discount { get; set; }

        public int? CategoryId { get; set; }
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();
        public List<int> ImageIdsToDelete { get; set; } = new List<int>();
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name_Ar { get; set; }
        public string Name_En { get; set; }
        public string Description { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Brand { get; set; }
        public string Warranty { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public bool IsFavorite { get; set; }
        public bool IsInCart { get; set; }
        public string? FirstImageUrl { get; set; }

        public decimal EffectivePrice { get; set; }          // السعر بعد الخصم (لو في خصم)
        public bool HasDiscount { get; set; }
        public int? AppliedBannerId { get; set; }
        public string? AppliedBannerTitle { get; set; }
        public decimal? DiscountValue { get; set; }
        public DiscountType? DiscountType { get; set; }
    }

    public class ProductImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
    }
}
