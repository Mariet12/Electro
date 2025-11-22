// Electro.Core/Interface/ICommunicationMethodsService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Electro.Core.Dtos;

namespace Electro.Core.Interface
{
    public interface ICommunicationMethodsService
    {
        Task<List<CommunicationMethodReadDto>> GetAllAsync();
        Task<CommunicationMethodReadDto?> GetAsync(int id);
        Task<int> CreateAsync(CommunicationMethodCreateDto dto);
        Task<bool> UpdateAsync(int id, CommunicationMethodCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
