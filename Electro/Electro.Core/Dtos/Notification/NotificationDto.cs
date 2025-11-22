namespace Electro.Core.Dtos
{
    public class UnreadCountsDto
    {
        public int UnreadNotificationsCount { get; set; }
    }

    public class SendRawNotificationDto
    {
        public string Token { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public Dictionary<string, string>? Data { get; set; }
        public string App { get; set; } = "Customer";
    }
}
