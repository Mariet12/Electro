// File: Core/Interface/INotificationService.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Electro.Core.Dtos;
using Electro.Core.Dtos.Notification;

namespace Electro.Core.Interface
{
    public interface INotificationService
    {
        // Reads (قوائم/عدادات)
        Task<NotificationListResponseDto> GetMyAsync(
            string userId,
            NotificationQueryDto? query,
            CancellationToken ct = default);

        Task<IEnumerable<Electro.Core.Models.Notification>> GetUserNotifications(string userId);
        Task<Electro.Core.Models.Notification?> GetNotificationById(int id);
        Task<(IEnumerable<Electro.Core.Models.Notification> Notifications, int UnreadCount)>
            GetUserNotificationsWithUnreadCount(string userId);

        // Create/Update/Delete
        Task AddNotification(string? senderId, string receiverId, string title, string message, string status, int? orderId);
        Task<bool> MarkAsReadAsync(int id, string userId);

        /// <summary>حذف إشعار (بشكل عام). استخدم Controller للتأكد من الملكية قبل النداء.</summary>
        Task DeleteNotification(int id);

        // Send (FCM)
        Task SendNotificationToAdmins(string senderId, string title, string message, string status, int? orderId);
        Task<string?> SendAndStoreAsync(string receiverId, string titleAr, string bodyAr, string status, int? orderId,
                                        string? senderId = null, Dictionary<string, string>? data = null);
        Task SendToUsersAsync(IEnumerable<string> receiverIds, string titleAr, string bodyAr, string status, int? orderId,
                              string? senderId = null, Dictionary<string, string>? data = null);
        Task<string?> SendFirebaseNotification(string fcmToken, string title, string body,
                                               Dictionary<string, string>? data = null, string app = "customer");

        // Counts
        Task<UnreadCountsDto> GetUnreadNotificationsCount(string userId);
        Task<int> MarkAllAsReadAsync(string userId, CancellationToken ct = default);

    }
}
