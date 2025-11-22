using Electro.Core.Dtos;

public interface IContactService
{
    // Public
    Task<int> SubmitInquiryAsync(ContactCreateDto dto);

    // Admin
    Task<PagedResult<ContactReadDto>> GetInquiriesAsync(int pageNumber, int pageSize,
        string? search, DateTime? fromUtc, DateTime? toUtc);

    Task<ContactReadDto?> GetInquiryAsync(int id);

    // الرد مرة واحدة
    Task<bool> ReplyAsync(int id, ContactInquiryReplyDto dto, string? adminUserId = null);

    Task<bool> DeleteInquiryAsync(int id);
}
