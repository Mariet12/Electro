// Electro.Service/ContactService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Electro.Core.Dtos;
using Electro.Core.Interface;
using Electro.Reposatory.Data.Identity;

namespace Electro.Service
{
    public class ContactService : IContactService
    {
        private readonly AppIdentityDbContext _ctx;
        public ContactService(AppIdentityDbContext ctx) => _ctx = ctx;

        // ===== Public =====
        public async Task<int> SubmitInquiryAsync(ContactCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException("Invalid payload");

            var e = new Contact
            {
                FullName = dto.FullName.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Email = dto.Email.Trim(),
                Message = dto.Message.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _ctx.Set<Contact>().Add(e);
            await _ctx.SaveChangesAsync();
            return e.Id;
        }

        // ===== Admin =====
        public async Task<PagedResult<ContactReadDto>> GetInquiriesAsync(
            int pageNumber, int pageSize,
            string? search, DateTime? fromUtc, DateTime? toUtc)
        {
            var from = (fromUtc ?? DateTime.UtcNow.AddDays(-60)).Date;
            var to = (toUtc ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);

            var q = _ctx.Set<Contact>().AsNoTracking()
                .Where(c => c.CreatedAt >= from && c.CreatedAt <= to);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(c =>
                    c.FullName.ToLower().Contains(s) ||
                    c.Email.ToLower().Contains(s) ||
                    c.PhoneNumber.ToLower().Contains(s) ||
                    c.Message.ToLower().Contains(s) ||
                    (c.AdminReply ?? "").ToLower().Contains(s));
            }

            var total = await q.CountAsync();

            var items = await q
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ContactReadDto
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    Message = c.Message,
                    AdminReply = c.AdminReply,
                    RepliedAt = c.RepliedAt,
                    RepliedByUserId = c.RepliedByUserId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<ContactReadDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        public async Task<ContactReadDto?> GetInquiryAsync(int id)
        {
            return await _ctx.Set<Contact>().AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ContactReadDto
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    Message = c.Message,
                    AdminReply = c.AdminReply,
                    RepliedAt = c.RepliedAt,
                    RepliedByUserId = c.RepliedByUserId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ReplyAsync(int id, ContactInquiryReplyDto dto, string? adminUserId = null)
        {
            var e = await _ctx.Set<Contact>().FirstOrDefaultAsync(c => c.Id == id);
            if (e == null) return false;

            e.AdminReply = dto.Reply.Trim();
            e.RepliedAt = DateTime.UtcNow;
            e.RepliedByUserId = adminUserId;
            e.UpdatedAt = DateTime.UtcNow;

            await _ctx.SaveChangesAsync();

            // اختياري: إرسال بريد بالرد
            // await _mailer.SendAsync(e.Email, "رد على استفسارك", e.AdminReply);

            return true;
        }

        public async Task<bool> DeleteInquiryAsync(int id)
        {
            var e = await _ctx.Set<Contact>().FirstOrDefaultAsync(c => c.Id == id);
            if (e == null) return false;

            _ctx.Set<Contact>().Remove(e);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
