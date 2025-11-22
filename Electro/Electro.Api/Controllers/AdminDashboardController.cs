using Electro.Core.Dtos.AdminDashboard;
using Electro.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electro.API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    //[Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _svc;
        public AdminDashboardController(IAdminDashboardService svc) => _svc = svc;

        private (DateTime fromUtc, DateTime toUtc) Normalize(DateTime? from, DateTime? to)
        {
            var toUtc = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
            var fromUtc = (from ?? DateTime.UtcNow.AddDays(-30)).Date;
            return (fromUtc, toUtc);
        }

        // البطاقات العلوية
        [HttpGet("summary")]
        public async Task<ActionResult> GetSummary([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var (f, t) = Normalize(from, to);
            var data = await _svc.GetSummaryAsync(f, t);
            return Ok(new { statusCode = 200, message = "summary", data });
        }

        // آخر الطلبات (افتراضيًا 5)
        [HttpGet("recent-orders")]
        public async Task<ActionResult> GetRecentOrders([FromQuery] int take = 5)
        {
            take = Math.Clamp(take, 1, 20);
            var data = await _svc.GetRecentOrdersAsync(take);
            return Ok(new { statusCode = 200, message = "recent orders", data });
        }

        // سلسلة المبيعات اليومية
        [HttpGet("sales-series")]
        public async Task<ActionResult> GetSalesSeries([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var (f, t) = Normalize(from, to);
            var data = await _svc.GetSalesSeriesAsync(f, t);
            return Ok(new { statusCode = 200, message = "sales series", data });
        }

        // توزيع الحالات
        [HttpGet("orders-status")]
        public async Task<ActionResult> GetStatusBreakdown([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var (f, t) = Normalize(from, to);
            var data = await _svc.GetOrdersStatusBreakdownAsync(f, t);
            return Ok(new { statusCode = 200, message = "status breakdown", data });
        }

        // أعلى المنتجات
        [HttpGet("top-products")]
        public async Task<ActionResult> GetTopProducts([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] int take = 5)
        {
            take = Math.Clamp(take, 1, 20);
            var (f, t) = Normalize(from, to);
            var data = await _svc.GetTopProductsAsync(f, t, take);
            return Ok(new { statusCode = 200, message = "top products", data });
        }

        // ملخص الفئات
        [HttpGet("categories-summary")]
        public async Task<ActionResult> GetCategoriesSummary()
        {
            var data = await _svc.GetCategoriesSummaryAsync();
            return Ok(new { statusCode = 200, message = "categories summary", data });
        }

        // إحصاءات الدفع
        [HttpGet("payments-stats")]
        public async Task<ActionResult> GetPaymentsStats([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var (f, t) = Normalize(from, to);
            var data = await _svc.GetPaymentsStatsAsync(f, t);
            return Ok(new { statusCode = 200, message = "payments stats", data });
        }

        // جدول الطلبات (للصفحة /orders)
        [HttpGet("orders")]
        public async Task<ActionResult> GetOrdersPage(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var (f, t) = Normalize(from, to);
            var data = await _svc.GetOrdersPageAsync(f, t, pageNumber, pageSize);
            return Ok(new { statusCode = 200, message = "orders page", data });
        }
        [HttpGet("customers")]
        public async Task<ActionResult<PagedResult<AdminCustomerRowDto>>> GetCustomers(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 20,
       [FromQuery] string? search = null,
       [FromQuery] string? sortBy = null,
       [FromQuery] bool desc = true,
       [FromQuery] DateTime? fromUtc = null,
       [FromQuery] DateTime? toUtc = null)
        {
            var result = await _svc.GetCustomersPageAsync(search, sortBy, desc, pageNumber, pageSize, fromUtc, toUtc);
            return Ok(new { statusCode = 200, message = "data", data = result });
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<AdminCustomerDetailsDto>> GetCustomer(string userId, [FromQuery] DateTime? fromUtc = null, [FromQuery] DateTime? toUtc = null)
        {
            var dto = await _svc.GetCustomerDetailsAsync(userId, fromUtc, toUtc);
            if (dto == null) return NotFound();
            return Ok(new {statusCode = 200,message = "data",data = dto });
        }
    }
}
