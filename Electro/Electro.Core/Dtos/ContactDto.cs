// Electro.Core/Dtos/ContactDtos.cs
namespace Electro.Core.Dtos
{
    // العميل يرسل السؤال
    public class ContactCreateDto
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    // عرض/إدارة السؤال
    public class ContactReadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? AdminReply { get; set; }
        public DateTime? RepliedAt { get; set; }
        public string? RepliedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // رد الأدمن (إجابة واحدة)
    public class ContactInquiryReplyDto
    {
        public string Reply { get; set; } = null!;
        // لو عايز تزوّد خصائص تانية لاحقًا، ضيف هنا
    }
}
