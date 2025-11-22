// Electro.Service/CommunicationMethodsService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Electro.Core.Interface;
using Electro.Reposatory.Data.Identity;
using Electro.Core.Dtos;
using Electro.Core.Models;

namespace Electro.Service
{
    public class CommunicationMethodsService : ICommunicationMethodsService
    {
        private readonly AppIdentityDbContext _ctx;
        public CommunicationMethodsService(AppIdentityDbContext ctx) => _ctx = ctx;

        public async Task<List<CommunicationMethodReadDto>> GetAllAsync()
        {
            var q = _ctx.CommunicationMethods.AsNoTracking().AsQueryable();

            return await q
                .Select(x => new CommunicationMethodReadDto
                {
                    Id = x.Id,
                    Address = x.Address,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    UrlLocation = x.UrlLocation,
                }).ToListAsync();
        }

        public async Task<CommunicationMethodReadDto?> GetAsync(int id)
        {
            return await _ctx.CommunicationMethods.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CommunicationMethodReadDto
                {
                    Id = x.Id,
                    Address = x.Address,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    UrlLocation = x.UrlLocation,
                }).FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(CommunicationMethodCreateDto dto)
        {
            // (تحقق بسيط – تقدر تزود Regex للهاتف/الإيميل)
            var e = new CommunicationMethods
            {
                Address = dto.Address.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Email = dto.Email.Trim(),
                UrlLocation = dto.UrlLocation.Trim(),
            };
            _ctx.CommunicationMethods.Add(e);
            await _ctx.SaveChangesAsync();
            return e.Id;
        }

        public async Task<bool> UpdateAsync(int id, CommunicationMethodCreateDto dto)
        {
            var e = await _ctx.CommunicationMethods.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return false;

            e.Address = dto.Address.Trim();
            e.PhoneNumber = dto.PhoneNumber.Trim();
            e.Email = dto.Email.Trim();
            e.UrlLocation = dto.UrlLocation.Trim();

            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _ctx.CommunicationMethods.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return false;
            _ctx.CommunicationMethods.Remove(e);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
