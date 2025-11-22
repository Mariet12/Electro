// File: Core/Dtos/Notifications/NotificationQueryDto.cs
namespace Electro.Core.Dtos.Notification
{
    public class NotificationQueryDto
    {
        public bool UnreadOnly { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class NotificationListItemDto
    {
        public int Id { get; set; }
        public string? SenderId { get; set; }
        public string ReceiverId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string Status { get; set; } = "General";
        public int? OrderId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationListResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public List<NotificationListItemDto> Items { get; set; } = new();
    }
}
