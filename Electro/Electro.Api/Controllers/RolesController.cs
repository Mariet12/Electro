using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Electro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // ✅ Get All Roles
        [HttpGet]
        public IActionResult GetAll()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        // ✅ Get Role By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        // ✅ Create Role
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("اسم الرول مطلوب");

            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists) return BadRequest("الرول موجود بالفعل");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                return Ok("تم إنشاء الرول بنجاح");

            return BadRequest(result.Errors);
        }

        // ✅ Update Role Name
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] string newRoleName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            role.Name = newRoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
                return Ok("تم تحديث اسم الرول");

            return BadRequest(result.Errors);
        }

        // ✅ Delete Role
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return Ok("تم حذف الرول");

            return BadRequest(result.Errors);
        }
    }

}
