// File: Service/NotificationService.cs
using Electro.Core.Dtos;
using Electro.Core.Dtos.Notification;
using Electro.Core.Interface;
using Electro.Core.Models;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace Electro.Service
{
    public class NotificationService : INotificationService
    {
        private readonly AppIdentityDbContext _context;

        public NotificationService(AppIdentityDbContext context)
        {
            _context = context;
        }

        // ====== Reads ======
        public async Task<NotificationListResponseDto> GetMyAsync(
            string userId,
            NotificationQueryDto? query,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("userId is required.", nameof(userId));

            var page = Math.Max(1, (query?.Page).GetValueOrDefault(1));
            var pageSize = Math.Clamp((query?.PageSize).GetValueOrDefault(20), 1, 100);
            var unreadOnly = (query?.UnreadOnly).GetValueOrDefault(false);

            var q = _context.Notifications.AsNoTracking().Where(n => n.ReceiverId == userId);
            if (unreadOnly) q = q.Where(n => !n.IsRead);

            var total = await q.CountAsync(ct);

            var items = await q
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationListItemDto
                {
                    Id = n.Id,
                    SenderId = n.SenderId,
                    ReceiverId = n.ReceiverId,
                    Title = n.Title,
                    Message = n.Message,
                    Status = n.Status,
                    OrderId = n.OrderId,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync(ct);

            var unreadCount = unreadOnly
                ? total
                : await _context.Notifications.AsNoTracking()
                    .Where(n => n.ReceiverId == userId && !n.IsRead)
                    .CountAsync(ct);

            return new NotificationListResponseDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                UnreadCount = unreadCount,
                Items = items
            };
        }

        public async Task<IEnumerable<Core.Models.Notification>> GetUserNotifications(string userId)
        {
            _ = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new Exception("المستخدم غير موجود");

            return await _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public Task<Core.Models.Notification?> GetNotificationById(int id)
            => _context.Notifications.FindAsync(id).AsTask();

        public async Task<(IEnumerable<Core.Models.Notification> Notifications, int UnreadCount)>
            GetUserNotificationsWithUnreadCount(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return (notifications, notifications.Count(n => !n.IsRead));
        }

        // ====== Create / Update / Delete ======
        public async Task AddNotification(string? senderId, string receiverId, string title, string message, string status, int? orderId)
        {
            var n = new Core.Models.Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Title = title,
                Message = message,
                Status = status,
                OrderId = orderId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _context.Notifications.Add(n);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> MarkAsReadAsync(int id, string userId)
        {
            var n = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.ReceiverId == userId);
            if (n == null) return false;
            n.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> MarkAllAsReadAsync(string userId, CancellationToken ct = default)
        {
            var notifs = await _context.Notifications
                .Where(n => n.ReceiverId == userId && !n.IsRead)
                .ToListAsync(ct);

            if (notifs.Count == 0) return 0;

            foreach (var n in notifs)
                n.IsRead = true;

            return await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteNotification(int id)
        {
            var n = await _context.Notifications.FindAsync(id);
            if (n != null)
            {
                _context.Notifications.Remove(n);
                await _context.SaveChangesAsync();
            }
        }

        // ====== Send (DB only, no Firebase) ======
        public async Task SendNotificationToAdmins(string senderId, string title, string message, string status, int? orderId)
        {
            var admins = await _context.Users.Where(u => u.Role == "Admin").ToListAsync();

            var now = DateTime.UtcNow;
            _context.Notifications.AddRange(
                admins.Select(a => new Core.Models.Notification
                {
                    SenderId = senderId,
                    ReceiverId = a.Id,
                    Title = title,
                    Message = message,
                    Status = status,
                    OrderId = orderId,
                    CreatedAt = now,
                    IsRead = false
                })
            );
            await _context.SaveChangesAsync();
        }

        public async Task<string?> SendAndStoreAsync(
            string receiverId,
            string titleAr,
            string bodyAr,
            string status,
            int? orderId,
            string? senderId = null,
            Dictionary<string, string>? data = null)
        {
            var n = new Core.Models.Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Title = titleAr,
                Message = bodyAr,
                Status = status,
                OrderId = orderId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _context.Notifications.Add(n);
            await _context.SaveChangesAsync();

            return n.Id.ToString();
        }

        public async Task SendToUsersAsync(
            IEnumerable<string> receiverIds,
            string titleAr,
            string bodyAr,
            string status,
            int? orderId,
            string? senderId = null,
            Dictionary<string, string>? data = null)
        {
            var ids = receiverIds?.Distinct().ToList() ?? new List<string>();
            if (ids.Count == 0) return;

            var now = DateTime.UtcNow;
            var rows = ids.Select(rid => new Core.Models.Notification
            {
                SenderId = senderId,
                ReceiverId = rid,
                Title = titleAr,
                Message = bodyAr,
                Status = status,
                OrderId = orderId,
                CreatedAt = now,
                IsRead = false
            }).ToList();

            _context.Notifications.AddRange(rows);
            await _context.SaveChangesAsync();
        }

        public Task<string?> SendFirebaseNotification(
            string fcmToken, string title, string body,
            Dictionary<string, string>? data = null, string app = "customer")
        {
            // تم إلغاء الفايربيز: مجرد null
            return Task.FromResult<string?>(null);
        }

        // ====== Unread counts ======
        public async Task<UnreadCountsDto> GetUnreadNotificationsCount(string userId)
        {
            var unreadNotificationsCount = await _context.Notifications
                .Where(n => n.ReceiverId == userId && !n.IsRead)
                .CountAsync();

            return new UnreadCountsDto { UnreadNotificationsCount = unreadNotificationsCount };
        }
    }
}
