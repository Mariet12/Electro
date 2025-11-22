// Electro.API/Controllers/AdminCommunicationMethodsController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Electro.Core.Interface;
using Electro.Core.Dtos;

namespace Electro.API.Controllers
{
    [ApiController]
    [Route("api/admin/communication-methods")]
    //[Authorize(Roles = "Admin")]
    public class AdminCommunicationMethodsController : ControllerBase
    {
        private readonly ICommunicationMethodsService _svc;
        public AdminCommunicationMethodsController(ICommunicationMethodsService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var data = await _svc.GetAllAsync();
            return Ok(new { statusCode = 200, message = "list", data });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var item = await _svc.GetAsync(id);
            if (item == null) return NotFound();
            return Ok(new { statusCode = 200, message = "item", data = item });
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CommunicationMethodCreateDto dto)
        {
            var id = await _svc.CreateAsync(dto);
            return Ok(new { statusCode = 200, message = "created", data = new { id } });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] CommunicationMethodCreateDto dto)
        {
            var ok = await _svc.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return Ok(new { statusCode = 200, message = "updated" });
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _svc.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok(new { statusCode = 200, message = "deleted" });
        }
    }
}
