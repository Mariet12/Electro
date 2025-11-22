using Electro.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Banner
{
    public class CreateBannerDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public BannerScope Scope { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public List<int>? ProductIds { get; set; }
        public List<int>? CategoryIds { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class BannerDto : CreateBannerDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }

}
