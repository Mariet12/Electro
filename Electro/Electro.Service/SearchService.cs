using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Electro.Core.Dtos.Search;
using Electro.Core.Interface;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Electro.Core.Services
{
    public class SearchService : ISearchService
    {
        private readonly AppIdentityDbContext _db;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;

        public SearchService(AppIdentityDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _baseUrl = (_config["BaseUrl"] ?? string.Empty).TrimEnd('/');
        }

        public async Task<PagedResult<SearchHitDto>> SearchAsync(
            SearchQueryDto query,
            string? requesterUserId,   // ✅ nullable الآن
            bool requesterIsAdmin,
            CancellationToken ct = default)
        {
            var q = (query.Q ?? string.Empty).Trim();
            if (q.Length < 1) throw new ArgumentException("Query too short");

            var tokens = Tokenize(q);
            var areas = (query.Areas == null || query.Areas.Length == 0)
                ? new[] { "products", "categories" }
                : query.Areas.Select(a => a.Trim().ToLowerInvariant()).Distinct().ToArray();

            var perAreaTake = Math.Max(10, query.PageSize);

            var hits = new List<SearchHitDto>();
            var countsByType = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // ========== PRODUCTS ==========
            if (areas.Contains("products"))
            {
                var pQuery = _db.Products.AsNoTracking()
                                         .Include(p => p.Category)
                                         .Where(p => !p.IsDeleted);

                foreach (var term in tokens)
                {
                    var t = term;
                    pQuery = pQuery.Where(p =>
                        (p.Name_En != null && p.Name_En.Contains(t)) ||
                        (p.Name_Ar != null && p.Name_Ar.Contains(t)) ||
                        (p.Description != null && p.Description.Contains(t)) ||
                        (p.Brand != null && p.Brand.Contains(t)));
                }

                countsByType["product"] = await pQuery.CountAsync(ct);

                var pRaw = await pQuery
                    .Select(p => new
                    {
                        p.Id,
                        p.Name_En,
                        p.Name_Ar,
                        p.Description,
                        p.Brand,
                        p.Price,
                        CategoryName = p.Category != null ? p.Category.Name : null,
                        FirstImageUrl = p.ProductImages.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault()
                    })
                    .Take(perAreaTake * 2)
                    .ToListAsync(ct);

                var pItems = pRaw
                    .Select(p => new SearchHitDto
                    {
                        Type = "product",
                        Id = p.Id.ToString(),
                        Title = (p.Name_En ?? p.Name_Ar) ?? $"Product #{p.Id}",
                        Snippet = MakeSnippet(p.Description, tokens),
                        Url = ApiUrl($"products/{p.Id}"),
                        Score = ScoreProduct(p.Name_En, p.Name_Ar, p.Brand, p.Description, tokens),
                        Meta = new Dictionary<string, object>
                        {
                            ["price"] = p.Price,
                            ["brand"] = p.Brand ?? "",
                            ["category"] = p.CategoryName ?? "",
                            ["firstImage"] = string.IsNullOrEmpty(p.FirstImageUrl) ? "" : AbsUrl(p.FirstImageUrl)
                        }
                    })
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Id)
                    .Take(perAreaTake)
                    .ToList();

                hits.AddRange(pItems);
            }

            // ========== CATEGORIES ==========
            if (areas.Contains("categories"))
            {
                var cQuery = _db.Categories.AsNoTracking();

                foreach (var term in tokens)
                {
                    var t = term;
                    cQuery = cQuery.Where(c => c.Name.Contains(t));
                }

                countsByType["category"] = await cQuery.CountAsync(ct);

                var cRaw = await cQuery
                    .Select(c => new { c.Id, c.Name })
                    .Take(perAreaTake * 2)
                    .ToListAsync(ct);

                var cItems = cRaw
                    .Select(c => new SearchHitDto
                    {
                        Type = "category",
                        Id = c.Id.ToString(),
                        Title = c.Name,
                        Url = ApiUrl($"Category/{c.Id}"),
                        Score = ScoreSimple(c.Name, tokens) + 0.1
                    })
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Id)
                    .Take(perAreaTake)
                    .ToList();

                hits.AddRange(cItems);
            }

            // ========== ORDERS (admin only) ==========
            if (areas.Contains("orders") && requesterIsAdmin)
            {
                var oQuery = _db.Orders.AsNoTracking();

                foreach (var term in tokens)
                {
                    var t = term;
                    oQuery = oQuery.Where(o =>
                        o.OrderNumber.Contains(t) ||
                        o.FullName.Contains(t) ||
                        (o.Email != null && o.Email.Contains(t)) ||
                        o.PhoneNumber.Contains(t) ||
                        o.ShippingAddress.Contains(t));
                }

                countsByType["order"] = await oQuery.CountAsync(ct);

                var oRaw = await oQuery
                    .Select(o => new
                    {
                        o.Id,
                        o.OrderNumber,
                        o.FullName,
                        o.Email,
                        o.PhoneNumber,
                        o.ShippingAddress,
                        o.Status,
                        o.TotalAmount,
                        o.CreatedAt
                    })
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(perAreaTake * 2)
                    .ToListAsync(ct);

                var oItems = oRaw
                    .Select(o => new SearchHitDto
                    {
                        Type = "order",
                        Id = o.Id.ToString(),
                        Title = $"{o.OrderNumber} — {o.FullName}",
                        Snippet = MakeSnippet($"{o.ShippingAddress} {o.Email} {o.PhoneNumber}", tokens),
                        Url = ApiUrl($"orders/{o.Id}"),
                        Score = ScoreOrder(o.OrderNumber, o.FullName, o.Email, o.ShippingAddress, tokens),
                        Meta = new Dictionary<string, object>
                        {
                            ["status"] = o.Status.ToString(),
                            ["total"] = o.TotalAmount,
                            ["createdAt"] = o.CreatedAt
                        }
                    })
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Id)
                    .Take(perAreaTake)
                    .ToList();

                hits.AddRange(oItems);
            }

            // ========== USERS (admin only) ==========
            if (areas.Contains("users") && requesterIsAdmin)
            {
                var uQuery = _db.Users.AsNoTracking();

                foreach (var term in tokens)
                {
                    var t = term;
                    uQuery = uQuery.Where(u =>
                        (u.UserName != null && u.UserName.Contains(t)) ||
                        (u.Email != null && u.Email.Contains(t)) ||
                        (u.PhoneNumber != null && u.PhoneNumber.Contains(t)));
                }

                countsByType["user"] = await uQuery.CountAsync(ct);

                var uRaw = await uQuery
                    .Select(u => new { u.Id, u.UserName, u.Email, u.PhoneNumber })
                    .Take(perAreaTake * 2)
                    .ToListAsync(ct);

                var uItems = uRaw
                    .Select(u => new SearchHitDto
                    {
                        Type = "user",
                        Id = u.Id,
                        Title = u.UserName ?? u.Email ?? $"User {u.Id}",
                        Snippet = MakeSnippet($"{u.Email} {u.PhoneNumber}", tokens),
                        Url = ApiUrl($"users/{u.Id}"),
                        Score = ScoreUser(u.UserName, u.Email, u.PhoneNumber, tokens)
                    })
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Id)
                    .Take(perAreaTake)
                    .ToList();

                hits.AddRange(uItems);
            }

            // ===== Merge & paginate =====
            var merged = hits
                .OrderByDescending(h => h.Score)
                .ThenByDescending(h => h.Id)
                .ToList();

            var total = countsByType.Values.DefaultIfEmpty(0).Sum();
            var skip = (query.Page - 1) * query.PageSize;
            var pageItems = merged.Skip(skip).Take(query.PageSize).ToList();

            return new PagedResult<SearchHitDto>
            {
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalCount = total,
                Items = pageItems,
                CountsByType = countsByType
            };
        }

        // ========= URL Helpers =========

        private string AbsUrl(string relativeOrAbsolute)
        {
            if (string.IsNullOrWhiteSpace(relativeOrAbsolute)) return relativeOrAbsolute ?? string.Empty;

            if (relativeOrAbsolute.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                relativeOrAbsolute.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return relativeOrAbsolute;

            var path = relativeOrAbsolute.TrimStart('/');
            if (string.IsNullOrEmpty(_baseUrl)) return "/" + path;
            return $"{_baseUrl}/{path}";
        }

        private string ApiUrl(string relative)
        {
            relative = relative?.TrimStart('/') ?? string.Empty;
            return AbsUrl($"api/{relative}");
        }

        // ========= Scoring / Tokenizing / Snippet =========

        private static string[] Tokenize(string q)
        {
            var tokens = Regex.Split(q.ToLowerInvariant(), @"[^\p{L}\p{Nd}]+")
                              .Where(t => t.Length >= 2)
                              .Take(6)
                              .ToArray();
            if (tokens.Length == 0) tokens = new[] { q.ToLowerInvariant() };
            return tokens;
        }

        private static double ScoreSimple(string? text, string[] tokens)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            var s = text.ToLowerInvariant();
            double sc = 0;
            foreach (var t in tokens)
            {
                if (string.IsNullOrEmpty(t)) continue;
                var idx = s.IndexOf(t, StringComparison.Ordinal);
                if (idx >= 0) sc += 1.0 + (Math.Max(0, 10 - idx) * 0.01);
            }
            return sc;
        }

        private static double ScoreProduct(string? nameEn, string? nameAr, string? brand, string? desc, string[] tokens)
        {
            double sc = 0;
            sc += ScoreSimple(nameEn, tokens) * 5.0;
            sc += ScoreSimple(nameAr, tokens) * 5.0;
            sc += ScoreSimple(brand, tokens) * 3.0;
            sc += ScoreSimple(desc, tokens) * 1.5;
            return sc;
        }

        private static double ScoreOrder(string? orderNumber, string? fullName, string? email, string? address, string[] tokens)
        {
            double sc = 0;
            sc += ScoreSimple(orderNumber, tokens) * 5.0;
            sc += ScoreSimple(fullName, tokens) * 2.0;
            sc += ScoreSimple(email, tokens) * 1.0;
            sc += ScoreSimple(address, tokens) * 1.0;
            return sc;
        }

        private static double ScoreUser(string? userName, string? email, string? phone, string[] tokens)
        {
            double sc = 0;
            sc += ScoreSimple(userName, tokens) * 3.0;
            sc += ScoreSimple(email, tokens) * 2.0;
            sc += ScoreSimple(phone, tokens) * 1.0;
            return sc;
        }

        private static string? MakeSnippet(string? text, string[] tokens, int radius = 90)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var s = text.Trim();
            var lower = s.ToLowerInvariant();

            int firstIdx = -1;
            foreach (var t in tokens)
            {
                if (string.IsNullOrEmpty(t)) continue;
                var i = lower.IndexOf(t, StringComparison.Ordinal);
                if (i >= 0 && (firstIdx == -1 || i < firstIdx)) firstIdx = i;
            }
            if (firstIdx < 0)
                return s.Length <= 2 * radius ? s : s.Substring(0, 2 * radius) + "…";

            var start = Math.Max(0, firstIdx - radius);
            var end = Math.Min(s.Length, firstIdx + tokens[0].Length + radius);
            var snippet = s.Substring(start, end - start);
            return (start > 0 ? "… " : "") + snippet + (end < s.Length ? " …" : "");
        }
    }
}
