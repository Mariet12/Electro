using System;

namespace Electro.Core.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string? SenderId { get; set; }
        public string ReceiverId { get; set; } = default!;

        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string Status { get; set; } = "General";

        public int? OrderId { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
