
using Electro.Core.Dtos;
using Electro.Core.Dtos.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBook.Core.Interface
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<PaginatedResult<CategoryDto>> GetAllAsync(string search, int page, int pageSize);
        Task<bool> CreateAsync(CategoryCreateDto dto, string baseUrl);
        Task<bool> UpdateAsync(int id, CategoryUpdateDto dto, string baseUrl);
        Task<bool> DeleteAsync(int id);
    }
}
