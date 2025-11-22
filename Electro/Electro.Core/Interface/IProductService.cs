using Electro.Core.Dtos;
using Electro.Core.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Interface
{
  
        public interface IProductService
        {
            Task<PagedResult<ProductDto>> GetAsync(ProductQuery query, CancellationToken ct = default);
            Task<ProductDto?> GetByIdAsync(int id, string? userId = null, CancellationToken ct = default);
            Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
            Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
            Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<ProductDto>> GetLatestAsync(string? userId, int take = 10, CancellationToken ct = default);
        Task<IReadOnlyList<ProductDto>> GetBestSellingAsync(string? userId, int take = 10, int? days = null, CancellationToken ct = default);

    }

}
