// Electro.Core/Entities/Contact/ContactInquiry.cs
public class Contact
{
    public int Id { get; set; }

    // بيانات العميل
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;


    // السؤال
    public string Message { get; set; } = null!;

    // الرد (إجابة واحدة فقط)
    public string? AdminReply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public string? RepliedByUserId { get; set; }  // اختياري لو عندك Users

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
