using Electro.Core.Dtos.AdminDashboard;
using Electro.Core.Dtos.Checkout;
using Electro.Core.Interface;
using Electro.Core.Models;
using Electro.Core.Models.Identity;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace Electro.Service
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly AppIdentityDbContext _ctx;

        public AdminDashboardService(AppIdentityDbContext ctx) => _ctx = ctx;

        // نفس الدالة، ونستخدمها فعليًا تحت
        private static (DateTime fromUtc, DateTime toUtc) Normalize(DateTime? fromUtc, DateTime? toUtc)
        {
            var to = (toUtc ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
            var from = (fromUtc ?? DateTime.UtcNow.AddDays(-30)).Date;
            return (from, to);
        }

        // خليها تقبل Nullable وتطبّق Normalize
        public async Task<SummaryCardDto> GetSummaryAsync(DateTime? fromUtc, DateTime? toUtc)
        {
            var (from, to) = Normalize(fromUtc, toUtc);

            var ordersInRange = _ctx.Orders.AsNoTracking()
                .Where(o => o.CreatedAt >= from && o.CreatedAt <= to);

            var paidAmount = await ordersInRange
                .Where(o => o.PaymentStatus)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var completedAmount = await ordersInRange
                .Where(o => o.Status == OrderStatus.Completed)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var ordersCount = await ordersInRange.CountAsync();

            // إجمالي العملاء (أو لو حابب داخل الفترة، غيّر الاستعلام)
            var customersCount = await _ctx.Users.CountAsync();

            return new SummaryCardDto
            {
                PaidAmount = paidAmount,
                CompletedSales = completedAmount,
                OrdersCount = ordersCount,
                CustomersCount = customersCount
            };
        }

        public async Task<IReadOnlyList<RecentOrderRowDto>> GetRecentOrdersAsync(int take)
        {
            // نعتمد على اسم العميل ورقمه المخزّن في الطلب؛
            // ونستخدم Left Join على Users كـ fallback للإيميل لو الاسم مش موجود
            var q =
                (from o in _ctx.Orders.AsNoTracking()
                 join u in _ctx.Users.AsNoTracking() on o.UserId equals u.Id into gj
                 from u in gj.DefaultIfEmpty()
                 orderby o.CreatedAt descending
                 select new RecentOrderRowDto
                 {
                     OrderId = o.Id,
                     OrderNumber = o.OrderNumber,
                     Customer = !string.IsNullOrEmpty(o.FullName)
                                ? (string.IsNullOrEmpty(o.PhoneNumber) ? o.FullName : $"{o.FullName} ({o.PhoneNumber})")
                                : (u.Email ?? o.UserId),
                     CreatedAt = o.CreatedAt,
                     Total = o.TotalAmount,
                     Status = o.Status.ToString(),
                     PaymentStatus = o.PaymentStatus
                 })
                .Take(take);

            var list = await q.ToListAsync();
            list.ForEach(r => r.Status = StatusText(ParseStatus(r.Status)));
            return list;
        }

        public async Task<IReadOnlyList<SalesPointDto>> GetSalesSeriesAsync(DateTime? fromUtc, DateTime? toUtc)
        {
            var (from, to) = Normalize(fromUtc, toUtc);

            var q = _ctx.Orders.AsNoTracking()
                .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= from && o.CreatedAt <= to)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new SalesPointDto { Date = g.Key, Amount = g.Sum(o => o.TotalAmount) })
                .OrderBy(p => p.Date);

            return await q.ToListAsync();
        }

        public async Task<StatusBreakdownDto> GetOrdersStatusBreakdownAsync(DateTime? fromUtc, DateTime? toUtc)
        {
            var (from, to) = Normalize(fromUtc, toUtc);

            var q = await _ctx.Orders.AsNoTracking()
                .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return new StatusBreakdownDto
            {
                Pending = q.Where(x => x.Status == OrderStatus.Pending).Select(x => x.Count).FirstOrDefault(),
                InProcessing = q.Where(x => x.Status == OrderStatus.InProcessing).Select(x => x.Count).FirstOrDefault(),
                Completed = q.Where(x => x.Status == OrderStatus.Completed).Select(x => x.Count).FirstOrDefault(),
                Cancelled = q.Where(x => x.Status == OrderStatus.Cancelled).Select(x => x.Count).FirstOrDefault(),
            };
        }

        public async Task<IReadOnlyList<TopProductDto>> GetTopProductsAsync(DateTime? fromUtc, DateTime? toUtc, int take)
        {
            var (from, to) = Normalize(fromUtc, toUtc);

            var q = _ctx.OrderItems.AsNoTracking()
                .Where(oi => oi.Order.CreatedAt >= from && oi.Order.CreatedAt <= to && oi.Order.Status == OrderStatus.Completed)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name_En, oi.Product.Name_Ar })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.Name_En ?? g.Key.Name_Ar,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.TotalPrice),
                    ImageUrl = g.Select(x => x.Product.ProductImages
                                            .OrderBy(i => i.Id)
                                            .Select(i => i.ImageUrl)
                                            .FirstOrDefault())
                                .FirstOrDefault()
                })
                .OrderByDescending(x => x.QuantitySold)
                .ThenByDescending(x => x.Revenue)
                .Take(take);

            return await q.ToListAsync();
        }

        public async Task<IReadOnlyList<CategorySummaryRowDto>> GetCategoriesSummaryAsync()
        {
            // تقليل N+1: Left join ثم GroupBy لحساب عدد المنتجات غير المحذوفة لكل فئة
            var q =
                from c in _ctx.Categories.AsNoTracking()
                join p in _ctx.Products.AsNoTracking().Where(p => !p.IsDeleted) on c.Id equals p.CategoryId into gp
                from p in gp.DefaultIfEmpty()
                group p by new { c.Id, c.Name } into g
                orderby g.Key.Name
                select new CategorySummaryRowDto
                {
                    CategoryId = g.Key.Id,
                    CategoryName = g.Key.Name,
                    ProductsCount = g.Count(x => x != null)
                };

            return await q.ToListAsync();
        }

        public async Task<PaymentsStatsDto> GetPaymentsStatsAsync(DateTime? fromUtc, DateTime? toUtc)
        {
            var (from, to) = Normalize(fromUtc, toUtc);

            // استعلام واحد بـ GroupBy بدل استعلامين
            var grouped = await _ctx.Orders.AsNoTracking()
                .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
                .GroupBy(o => o.PaymentStatus)
                .Select(g => new { Paid = g.Key, Count = g.Count(), Amount = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            var paid = grouped.FirstOrDefault(x => x.Paid == true);
            var unpaid = grouped.FirstOrDefault(x => x.Paid == false);

            return new PaymentsStatsDto
            {
                PaidAmount = paid?.Amount ?? 0m,
                UnpaidAmount = unpaid?.Amount ?? 0m,
                PaidOrders = paid?.Count ?? 0,
                UnpaidOrders = unpaid?.Count ?? 0
            };
        }

        public async Task<PagedResult<RecentOrderRowDto>> GetOrdersPageAsync(
       DateTime? fromUtc, DateTime? toUtc, int pageNumber, int pageSize)
        {
            var (fromDate, toDate) = Normalize(fromUtc, toUtc);

            var baseQ = _ctx.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems) // ✅ عشان يجيب الـ OrderItems
                .GroupJoin(_ctx.Users.AsNoTracking(),
                    o => o.UserId,
                    u => u.Id,
                    (o, users) => new { o, users })
                .SelectMany(
                    x => x.users.DefaultIfEmpty(),
                    (x, u) => new { x.o, u });

            // ===== Count =====
            var total = await baseQ
                .Where(x => x.o.CreatedAt >= fromDate && x.o.CreatedAt <= toDate)
                .CountAsync();

            // ===== Page =====
            var items = await baseQ
                .Where(x => x.o.CreatedAt >= fromDate && x.o.CreatedAt <= toDate)
                .OrderByDescending(x => x.o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new RecentOrderRowDto
                {
                    OrderId = x.o.Id,
                    OrderNumber = x.o.OrderNumber,
                    Customer = !string.IsNullOrEmpty(x.o.FullName)
                               ? (string.IsNullOrEmpty(x.o.PhoneNumber) ? x.o.FullName : $"{x.o.FullName} ({x.o.PhoneNumber})")
                               : (x.u.Email ?? x.o.UserId),
                    Email = x.o.Email,
                    PhoneNumber = x.o.PhoneNumber,
                    PaymentMethod = x.o.PaymentMethod,
                    ShippingAddress = x.o.ShippingAddress,
                    UpdatedAt = x.o.UpdatedAt,
                    UserId = x.o.UserId,
                    CreatedAt = x.o.CreatedAt,
                    Total = x.o.TotalAmount,
                    Status = x.o.Status.ToString(),
                    PaymentStatus = x.o.PaymentStatus,
                    OrderItems = x.o.OrderItems // ✅ بقى يرجع في JSON
                })
                .ToListAsync();

            // تحويل الـ Status للنص المطلوب
            items.ForEach(r => r.Status = StatusText(ParseStatus(r.Status)));

            return new PagedResult<RecentOrderRowDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        public async Task<PagedResult<AdminCustomerRowDto>> GetCustomersPageAsync(
        string? search, string? sortBy, bool desc, int pageNumber, int pageSize,
        DateTime? fromUtc, DateTime? toUtc)
        {
            var (from, to) = Normalize(fromUtc, toUtc);

            // 1) قاعدة المستخدمين + فلترة نصية
            IQueryable<AppUser> usersQ = _ctx.Users.Where(d=>d.Role == "Customer").AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                usersQ = usersQ.Where(u =>
                    (u.FullName ?? "").ToLower().Contains(s) ||
                    (u.Email ?? "").ToLower().Contains(s) ||
                    (u.PhoneNumber ?? "").ToLower().Contains(s));
            }

            // إجمالي السجلات قبل التقسيم لصفحات
            var total = await usersQ.CountAsync();

            // 2) نبني Projection فيه إحصائيات بالـ Correlated Subqueries (مترجمة SQL)
            var itemsQ = usersQ.Select(u => new AdminCustomerRowDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,

                OrdersCount = _ctx.Orders
                    .Where(o => o.UserId == u.Id && o.CreatedAt >= from && o.CreatedAt <= to)
                    .Count(),

                TotalSpent = _ctx.Orders
                    .Where(o => o.UserId == u.Id && o.CreatedAt >= from && o.CreatedAt <= to)
                    .Sum(o => (decimal?)o.TotalAmount) ?? 0m,

                LastOrderAt = _ctx.Orders
                    .Where(o => o.UserId == u.Id && o.CreatedAt >= from && o.CreatedAt <= to)
                    .Max(o => (DateTime?)o.CreatedAt),

                IsActive = u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow
            });

            // 3) فرز ديناميكي على الحقول المحسوبة
            itemsQ = (sortBy?.ToLower(), desc) switch
            {
                ("name", true) => itemsQ.OrderByDescending(x => x.FullName),
                ("name", false) => itemsQ.OrderBy(x => x.FullName),

                ("email", true) => itemsQ.OrderByDescending(x => x.Email),
                ("email", false) => itemsQ.OrderBy(x => x.Email),

                ("orders", true) => itemsQ.OrderByDescending(x => x.OrdersCount).ThenByDescending(x => x.TotalSpent),
                ("orders", false) => itemsQ.OrderBy(x => x.OrdersCount).ThenBy(x => x.TotalSpent),

                ("spent", true) => itemsQ.OrderByDescending(x => x.TotalSpent),
                ("spent", false) => itemsQ.OrderBy(x => x.TotalSpent),

                ("lastorder", true) => itemsQ.OrderByDescending(x => x.LastOrderAt),
                ("lastorder", false) => itemsQ.OrderBy(x => x.LastOrderAt),

                _ => itemsQ.OrderByDescending(x => x.LastOrderAt).ThenByDescending(x => x.TotalSpent)
            };

            // 4) Paging
            var items = await itemsQ
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AdminCustomerRowDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        public async Task<AdminCustomerDetailsDto?> GetCustomerDetailsAsync(string userId, DateTime? fromUtc, DateTime? toUtc)
        {
            var (from, to) = Normalize(fromUtc, toUtc);

            var user = await _ctx.Users.AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.Address,
                    u.City,
                    IsActive = u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow
                })
                .FirstOrDefaultAsync();

            if (user == null) return null;

            // إحصائياته في الفترة
            var stats = await _ctx.Orders.AsNoTracking()
                .Where(o => o.UserId == userId && o.CreatedAt >= from && o.CreatedAt <= to)
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    OrdersCount = g.Count(),
                    TotalSpent = g.Sum(x => x.TotalAmount),
                    LastOrderAt = g.Max(x => x.CreatedAt)
                })
                .FirstOrDefaultAsync();

            var orders = await _ctx.Orders.AsNoTracking()
        .Where(o => o.UserId == userId && o.CreatedAt >= from && o.CreatedAt <= to)
        .OrderByDescending(o => o.CreatedAt)
        .Take(50)
        .Select(o => new OrderDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            CreatedAt = o.CreatedAt,
            TotalAmount = o.TotalAmount,
            Status = o.Status,        // أو StatusText(o.Status) لو عايز عربي
            PaymentStatus = o.PaymentStatus,
            Email = o.Email,
            FullName = o.FullName,
            PaymentMethod = o.PaymentMethod,
            PhoneNumber = o.PhoneNumber,
            ShippingAddress = o.ShippingAddress,
            UpdatedAt = o.UpdatedAt,
            Items = o.OrderItems.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name_En ?? i.Product.Name_Ar,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                ProductImage = i.Product.ProductImages
               .OrderBy(pi => pi.Id)
               .Select(pi => pi.ImageUrl)
               .FirstOrDefault()
            }).ToList()
        })
        .ToListAsync();

            return new AdminCustomerDetailsDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                IsActive = user.IsActive,

                OrdersCount = stats?.OrdersCount ?? 0,
                TotalSpent = stats?.TotalSpent ?? 0m,
                LastOrderAt = stats?.LastOrderAt,
                Orders = orders

            };
        }


        // Placeholder كما هو
        public async Task<PagedResult<RecentOrderRowDto>> GetProductsPageAsync(int pageNumber, int pageSize)
        {
            return new PagedResult<RecentOrderRowDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0,
                Items = Array.Empty<RecentOrderRowDto>()
            };
        }

        // Helpers
        private static string StatusText(OrderStatus st) => st switch
        {
            OrderStatus.Pending => "في الانتظار",
            OrderStatus.InProcessing => "قيد التنفيذ",
            OrderStatus.Completed => "مكتمل",
            OrderStatus.Cancelled => "ملغي",
            _ => st.ToString()
        };

        private static OrderStatus ParseStatus(string s)
            => Enum.TryParse<OrderStatus>(s, out var st) ? st : OrderStatus.Pending;
    }
}
