
using eBook.Core.Interface;
using Electro.Core.Dtos.Category;
using Electro.Core.Errors;
using Microsoft.AspNetCore.Mvc;

namespace eBook.Apis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryController(ICategoryService categoryService, IHttpContextAccessor httpContextAccessor)
        {
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _categoryService.GetAllAsync(search, page, pageSize);
            return Ok(new ApiResponse(200, "Success", data));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse(404, "Category not found"));

            return Ok(new ApiResponse(200, "Success", category));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryCreateDto dto)
        {
            var baseUrl = GetBaseUrl();

            var created = await _categoryService.CreateAsync(dto, baseUrl);
            if (!created)
                return StatusCode(500, new ApiResponse(500, "Error creating category"));

            return Ok(new ApiResponse(200, "Category created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CategoryUpdateDto dto)
        {
            var baseUrl = GetBaseUrl();

            var updated = await _categoryService.UpdateAsync(id, dto, baseUrl);
            if (!updated)
                return NotFound(new ApiResponse(404, "Category not update"));

            return Ok(new ApiResponse(200, "Category updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponse(404, "Category not found or delete failed"));

            return Ok(new ApiResponse(200, "Category deleted successfully"));
        }
    }
}
