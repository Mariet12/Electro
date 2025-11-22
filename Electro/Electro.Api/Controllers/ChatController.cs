
using Electro.Core.Dtos.Chat;
using Electro.Core.Entities.Chat;
using Electro.Core.Interfaces;
using Electro.Core.Models.Identity;
using Electro.Reposatory.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Electro.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppIdentityDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext, AppIdentityDbContext context, IChatService chatService, UserManager<AppUser> userManager)
        {
            _chatService = chatService;
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("create-conversation")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDTO dto)
        {
            // إنشاء أو الحصول على المحادثة باستخدام الـ senderId و receiverId
            try
            {
                var conversation = await _chatService.CreateOrGetConversationAsync(dto.SenderId, dto.ReceiverId);

                // الحصول على بيانات المرسل والمستقبل
                var sender = await _userManager.Users.FirstOrDefaultAsync(d => d.Id == dto.SenderId);
                var recipient = await _userManager.Users.FirstOrDefaultAsync(d => d.Id == dto.ReceiverId);

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Conversation created successfully",
                    data = new
                    {
                        Id = conversation.Id,
                        SenderId = sender.Id,
                        ReceiverId = recipient.Id,
                        UserName = recipient.UserName,
                        UserImage = recipient.Image ?? "default_image_url"

                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("all-chats")]
        public async Task<IActionResult> GetAllConversationsAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            try
            {
                // تمرير الـ User إلى الخدمة للحصول على المحادثات
                var conversations = await _chatService.GetAllConversationsAsync(User);
                return Ok(new { statusCode = 200, message = "successfully", data = conversations }); // إرجاع المحادثات بصيغة JSON
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpGet("search-conversations")]
        public async Task<IActionResult> SearchConversationsAsync(string term)
        {
            try
            {
                // استدعاء الخدمة لاسترجاع المحادثات بناءً على الكلمة
                var conversations = await _chatService.GetAllConversationsAsync(User, term);

                // استجابة ناجحة
                return Ok(new
                {
                    StatusCode = 200,
                    message = "successfully.",
                    data = conversations
                });
            }
            catch (Exception ex)
            {
                // استجابة عند حدوث خطأ
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO dto)
        {
            // ✅ إرسال الرسالة واستقبال كائن الرسالة بعد الحفظ
            var message = await _chatService.SendMessageAsync(dto.ConversationId, dto.SenderId, dto.ReceiverId, dto.Content);

            var sender = await _userManager.Users.FirstOrDefaultAsync(d => d.Id == dto.SenderId);
            var recipient = await _userManager.Users.FirstOrDefaultAsync(d => d.Id == dto.ReceiverId);

            if (sender == null || recipient == null)
            {
                return BadRequest(new { StatusCode = 400, message = "Invalid sender or recipient" });
            }

            // إرسال الرسالة عبر SignalR
            await _hubContext.Clients.User(dto.ReceiverId)
                .SendAsync("ReceiveMessage", new
                {
                    conversationId = message.ConversationId,
                    messageDetails = new
                    {
                        sender = new
                        {
                            senderId = sender.Id,
                            senderName = sender.UserName,
                            senderImage = sender.Image ?? "default_image_url"
                        },
                        recipient = new
                        {
                            recipientId = recipient.Id,
                            recipientName = recipient.UserName,
                            recipientImage = recipient.Image ?? "default_image_url"
                        },
                        content = message.Content,
                        sentAt = message.SentAt
                    }
                });

            // إرجاع استجابة الـ API
            return Ok(new
            {
                StatusCode = 200,
                message = "Message sent successfully",
                data = new
                {
                    conversationId = message.ConversationId,
                    messageDetails = new
                    {
                        sender = new
                        {
                            senderId = sender.Id,
                            senderName = sender.UserName,
                            senderImage = sender.Image ?? "default_image_url"
                        },
                        recipient = new
                        {
                            recipientId = recipient.Id,
                            recipientName = recipient.UserName,
                            recipientImage = recipient.Image ?? "default_image_url"
                        },
                        content = message.Content,
                        sentAt = message.SentAt
                    }
                }
            });
        }
        [HttpGet("get-messages/{conversationId}")]
        public async Task<IActionResult> GetMessagesAsync(int conversationId)
        {
            if (conversationId <= 0)
            {
                return BadRequest(new { message = "ConversationId must be a valid number." });
            }

            try
            {
                var messages = await _chatService.GetMessagesAsync(conversationId);

                return Ok(new
                {
                    StatusCode = 200,
                    message = "successfully.",
                    data = messages
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("mark-all-messages-as-read")]
        public async Task<IActionResult> MarkAllMessagesAsRead([FromBody] MarkMessagesReadRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId) || request.ConversationId <= 0)
            {
                return BadRequest("Invalid parameters.");
            }

            var messages = await _context.Messages
                .Where(m => m.ConversationId == request.ConversationId && m.ReceiverId == request.UserId && !m.IsRead)
                .ToListAsync();

            if (messages.Any())
            {
                // تحديث حالة جميع الرسائل غير المقروءة إلى مقروءة
                foreach (var message in messages)
                {
                    message.IsRead = true;
                }

                // حفظ التحديثات في قاعدة البيانات
                await _context.SaveChangesAsync();

                // إرسال تحديث إلى SignalR لتحديث واجهة المستخدم
                await _hubContext.Clients.User(request.UserId).SendAsync("UpdateMessagesStatus", request.ConversationId, true);

                return Ok(new { message = "Messages marked as read successfully." });
            }

            return NotFound("No unread messages found in this conversation.");
        }
        [HttpGet("unread-chats-count")]
        public async Task<IActionResult> GetUnreadChatsCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }
            // استرجاع المحادثات التي تحتوي على رسائل غير مقروءة للمستخدم
            var unreadChatsCount = await _context.Conversations
                .Where(c => (c.SenderId == userId || c.ReceiverId == userId) &&
                            c.Messages.Any(m => m.ReceiverId == userId && !m.IsRead))
                .CountAsync();

            return Ok(new { unreadChatsCount });
        }
        public class MarkMessagesReadRequest
        {
            public int ConversationId { get; set; }
            public string UserId { get; set; }
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto messageDto)
        {
            // التحقق من صحة البيانات المدخلة
            if (string.IsNullOrEmpty(messageDto.SenderId) || string.IsNullOrEmpty(messageDto.ReceiverId) || messageDto.ConversationId <= 0)
            {
                return BadRequest(new { message = "Invalid input data." });
            }

            // حفظ الرسالة في قاعدة البيانات
            var message = new Message
            {
                ConversationId = messageDto.ConversationId,
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId,
                Content = messageDto.Message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // إرسال الرسالة عبر SignalR
            await _hubContext.Clients.User(messageDto.ReceiverId)
                .SendAsync("ReceiveMessage", messageDto.ConversationId, messageDto.SenderId, messageDto.ReceiverId, messageDto.Message, message.SentAt);

            return Ok(new
            {
                Status = "Message Sent",
                data = new
                {
                    messageId = message.Id,
                    conversationId = message.ConversationId,
                    senderId = message.SenderId,
                    receiverId = message.ReceiverId,
                    content = message.Content,
                    sentAt = message.SentAt
                }
            });
        }

        // تعديل كلاس ChatMessageDto ليشمل ConversationId
        public class ChatMessageDto
        {
            public int ConversationId { get; set; }  // معرف المحادثة
            public string SenderId { get; set; }
            public string ReceiverId { get; set; }
            public string Message { get; set; }
        }

    }
}
