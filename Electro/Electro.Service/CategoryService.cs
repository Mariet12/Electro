using eBook.Core.Interface;
using Electro.Core.Dtos;
using Electro.Core.Dtos.Category;
using Electro.Core.Interface;
using Electro.Core.Models;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBook.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppIdentityDbContext _context;
        private readonly IFileService _fileService;

        public CategoryService(AppIdentityDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<PaginatedResult<CategoryDto>> GetAllAsync(string? search, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var categories = _context.Categories.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = $"%{search.Trim()}%";
                categories = categories.Where(c => EF.Functions.Like(c.Name, s));
            }

            var totalCount = await categories.CountAsync();

            var items = await categories
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                .ToListAsync();

            // ✅ تتابعي لتفادي A second operation...
            var categoryCount = await _context.Categories.AsNoTracking().CountAsync();
            var productsCount = await _context.Products.AsNoTracking().CountAsync();

            return new PaginatedResult<CategoryDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Items = items,
                Extras = new
                {
                    CategoryCount = categoryCount,
                    ProductsCount = productsCount
                }
            };
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task<bool> CreateAsync(CategoryCreateDto dto, string baseUrl)
        {
           
            var category = new Category
            {
                Name = dto.Name,
            };

            _context.Categories.Add(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto, string baseUrl)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                category.Name = dto.Name;

           

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
