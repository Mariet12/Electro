namespace Electro.Core.Dtos.Search
{
    public class SearchQueryDto
    {
        public string Q { get; set; } = "";
        public string[]? Areas { get; set; } // ["products","categories","orders","users","chat"]
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class SearchHitDto
    {
        public string Type { get; set; } = "";         // product | category | order | user | chat
        public string Id { get; set; } = "";           // string علشان يدعم user id
        public string Title { get; set; } = "";
        public string? Snippet { get; set; }
        public string? Url { get; set; }               // لينك مناسب للفرونت
        public double Score { get; set; }
        public Dictionary<string, object>? Meta { get; set; }
    }

    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public Dictionary<string, int>? CountsByType { get; set; } // اختياري: عدد النتائج لكل نوع
    }
}
