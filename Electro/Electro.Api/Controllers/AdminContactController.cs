using Electro.Core.Dtos;
using Electro.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/contact")]
//[Authorize(Roles="Admin")]
public class AdminContactController : ControllerBase
{
    private readonly IContactService _svc;
    private readonly UserManager<AppUser> _userManager; // لو هتسجّل مين اللي رد

    public AdminContactController(IContactService svc, UserManager<AppUser> um)
    {
        _svc = svc; _userManager = um;
    }

    [HttpPost("submit")]
    public async Task<ActionResult> Submit([FromBody] ContactCreateDto dto)
    {
        var id = await _svc.SubmitInquiryAsync(dto);
        return Ok(new { statusCode = 200, message = "submitted", data = new { id } });
    }

    // (اختياري) العميل يراجع سؤاله ويرى الرد
    [HttpGet("{id:int}")]
    public async Task<ActionResult> Get(int id)
    {
        var item = await _svc.GetInquiryAsync(id);
        if (item == null) return NotFound();
        return Ok(new { statusCode = 200, message = "inquiry", data = item });
    }
    [HttpGet]
    public async Task<ActionResult> List(int pageNumber = 1, int pageSize = 20,
        string? search = null,
        DateTime? fromUtc = null, DateTime? toUtc = null)
    {
        var data = await _svc.GetInquiriesAsync(pageNumber, pageSize, search, fromUtc, toUtc);
        return Ok(new { statusCode = 200, message = "inquiries", data });
    }


    [HttpPost("{id:int}/reply")]
    public async Task<ActionResult> Reply(int id, ContactInquiryReplyDto dto)
    {
        string? adminId = User?.Identity?.IsAuthenticated == true
            ? _userManager.GetUserId(User) : null;

        var ok = await _svc.ReplyAsync(id, dto, adminId);
        if (!ok) return NotFound();
        return Ok(new { statusCode = 200, message = "replied" });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var ok = await _svc.DeleteInquiryAsync(id);
        if (!ok) return NotFound();
        return Ok(new { statusCode = 200, message = "deleted" });
    }
}
