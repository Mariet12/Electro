using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos
{
    public class PaginatedResult<T>
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
        public object? Extras { get; set; }

    }
    public class FilterDto
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public sealed class PaginationParams
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; init; } = 1;
        private int _pageSize = 20;
        public int PageSize { get => _pageSize; init => _pageSize = Math.Min(MaxPageSize, Math.Max(1, value)); }
    }

    public sealed class PagedResult<T>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    }

    public sealed class ProductQuery
    {
        public string? UserId { get; init; }           // عشان نرجّع IsInCart/IsFavorite
        public int? CategoryId { get; init; }
        public string? Search { get; init; }           // اسم عربي/إنجليزي/وصف/ماركة
        public string? Brand { get; init; }
        public double? PriceFrom { get; init; }
        public double? PriceTo { get; init; }
        public string? CountryOfOrigin { get; init; }
        public string? SortBy { get; init; } = "new";  // new | price | -price | name | -name
        public PaginationParams Paging { get; init; } = new();
    }

}
