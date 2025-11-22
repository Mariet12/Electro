using Electro.Core.Dtos.Checkout;
using Electro.Core.Dtos.Order;
using Electro.Core.Interface;
using Electro.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Electro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // كل الأكشنز تتطلب توكن؛ بنخصص رولز لأكشنز الأدمن
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;

        private string GetUserIdOrThrow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("Invalid token: userId not found.");
            return userId;
        }

        // العميل يعمل Checkout → Pending
        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var order = await _orderService.CheckoutAsync(userId, dto);
                return Ok(new { statusCode = 200, message = "order created", data = order });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { statusCode = 400, message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "حدث خطأ أثناء إنشاء الطلب" });
            }
        }

        // العميل يجيب أوردره
        [HttpGet("{orderId:int}")]
        public async Task<ActionResult> GetOrder(int orderId)
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var order = await _orderService.GetOrderAsync(orderId, userId);
                if (order == null)
                    return NotFound(new { statusCode = 404, message = "الطلب غير موجود" });

                return Ok(new { statusCode = 200, message = "order", data = order });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "حدث خطأ أثناء جلب الطلب" });
            }
        }

        // كل أوردرات المستخدم
        [HttpGet]
        public async Task<ActionResult> GetUserOrders()
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(new { statusCode = 200, message = "orders", data = orders });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "حدث خطأ أثناء جلب الطلبات" });
            }
        }

        // العميل يقدر يلغيه وهو Pending فقط
        [HttpPut("{orderId:int}/cancel")]
        public async Task<ActionResult> CancelOrder(int orderId)
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var result = await _orderService.CancelOrderAsync(orderId, userId);
                if (!result)
                    return BadRequest(new { statusCode = 400, message = "لا يمكن إلغاء هذا الطلب" });

                return Ok(new { statusCode = 200, message = "تم إلغاء الطلب بنجاح" });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "حدث خطأ أثناء إلغاء الطلب" });
            }
        }

        // ------- Admin only -------

        [HttpPut("{orderId:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(orderId, dto.Status);
                if (order == null)
                    return NotFound(new { statusCode = 404, message = "الطلب غير موجود" });

                return Ok(new { statusCode = 200, message = "status updated", data = order });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { statusCode = 400, message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "حدث خطأ أثناء تحديث حالة الطلب" });
            }
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllOrders(
            [FromQuery] OrderStatus? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(status, pageNumber, pageSize);
                return Ok(new { statusCode = 200, message = "all orders", data = orders });
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, message = "حدث خطأ أثناء جلب الطلبات" });
            }
        }

        [HttpPut("{orderId:int}/payment")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetPaymentStatus(int orderId, [FromBody] SetPaymentStatusDto dto)
        {
            var newStatus = await _orderService.SetPaymentStatusAsync(orderId, dto.IsPaid);
            if (newStatus == null)
                return NotFound(new { statusCode = 404, message = "الطلب غير موجود" });

            return Ok(new { statusCode = 200, message = "تم تحديث حالة الدفع", isPaid = newStatus });
        }

        [HttpPut("{orderId:int}/toggle-payment")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> TogglePaymentStatus(int orderId)
        {
            var newStatus = await _orderService.TogglePaymentStatusAsync(orderId);
            if (newStatus == null)
                return NotFound(new { statusCode = 404, message = "الطلب غير موجود" });

            return Ok(new { statusCode = 200, message = "تم تغيير حالة الدفع", isPaid = newStatus });
        }
    }
}
