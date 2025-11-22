using Electro.Core.Dtos.AdminDashboard;
using Electro.Core.Models;

namespace Electro.Core.Interface
{
    public interface IAdminDashboardService
    {
        Task<SummaryCardDto> GetSummaryAsync(DateTime? fromUtc, DateTime? toUtc);
        Task<IReadOnlyList<RecentOrderRowDto>> GetRecentOrdersAsync(int take);
        Task<IReadOnlyList<SalesPointDto>> GetSalesSeriesAsync(DateTime? fromUtc, DateTime? toUtc);
        Task<StatusBreakdownDto> GetOrdersStatusBreakdownAsync(DateTime? fromUtc, DateTime? toUtc);
        Task<IReadOnlyList<TopProductDto>> GetTopProductsAsync(DateTime? fromUtc, DateTime? toUtc, int take);
        Task<IReadOnlyList<CategorySummaryRowDto>> GetCategoriesSummaryAsync();
        Task<PaymentsStatsDto> GetPaymentsStatsAsync(DateTime? fromUtc, DateTime? toUtc);
        Task<PagedResult<RecentOrderRowDto>> GetOrdersPageAsync(DateTime? fromUtc, DateTime? toUtc, int pageNumber, int pageSize);
        Task<PagedResult<RecentOrderRowDto>> GetProductsPageAsync(int pageNumber, int pageSize); // للجدول placeholder
        Task<PagedResult<AdminCustomerRowDto>> GetCustomersPageAsync(string? search, string? sortBy, bool desc, int pageNumber, int pageSize,DateTime? fromUtc, DateTime? toUtc);
        Task<AdminCustomerDetailsDto?> GetCustomerDetailsAsync(string userId, DateTime? fromUtc, DateTime? toUtc);

    }
}
