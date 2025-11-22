using Electro.Core.Dtos.Checkout;
using Electro.Core.Models;

namespace Electro.Core.Dtos.AdminDashboard
{
    public sealed class DateRangeDto
    {
        public DateTime? From { get; set; }  // UTC
        public DateTime? To { get; set; }    // UTC
    }

    public sealed class SummaryCardDto
    {
        public decimal PaidAmount { get; set; }          // إجمالي المدفوع (PaymentStatus=true)
        public decimal CompletedSales { get; set; }      // إجمالي الطلبات المكتملة
        public int OrdersCount { get; set; }             // عدد الطلبات في الفترة
        public int CustomersCount { get; set; }          // إجمالي العملاء
    }

    public class RecentOrderRowDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Customer { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public bool PaymentStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public sealed class SalesPointDto
    {
        public DateTime Date { get; set; }   // بداية اليوم (UTC)
        public decimal Amount { get; set; }  // مجموع المبيعات في اليوم (Completed)
    }

    public sealed class StatusBreakdownDto
    {
        public int Pending { get; set; }
        public int InProcessing { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }

    public sealed class TopProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public string? ImageUrl { get; set; }
    }

    public sealed class CategorySummaryRowDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductsCount { get; set; }
    }

    public sealed class PaymentsStatsDto
    {
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public int PaidOrders { get; set; }
        public int UnpaidOrders { get; set; }
    }

    public sealed class PagedResult<T>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    }
    public class AdminCustomerRowDto
    {
        public string UserId { get; set; } = default!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        // إحصائيات
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderAt { get; set; }
        public bool IsActive { get; set; }   // مثال: نشط/غير نشط حسب نظامك
    }

    public class AdminCustomerDetailsDto : AdminCustomerRowDto
    {
        public string? Address { get; set; }   // لو عندك حقول إضافية
        public string? City { get; set; }
        public string? Country { get; set; }
        public List<OrderDto> Orders { get; set; } = new();

    }
}
