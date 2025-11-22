using Electro.Core.Dtos.Chat;
using Electro.Core.Entities.Chat;
using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Interfaces
{
    public interface IChatService
    {
        Task<Conversation> CreateOrGetConversationAsync(string senderId, string receiverId);
        Task<Message> SendMessageAsync(int conversationId, string senderId, string receiverId, string content);
        Task<IEnumerable<ConversationDTO>> GetAllConversationsAsync(ClaimsPrincipal user, string term = null);
        Task<List<MessageDto>> GetMessagesAsync(int conversationId);
        Task SaveMessageAsync(MessageDto messageDto);
        Task<IEnumerable<ConversationDTO>> GetAllConversationsAsync(ClaimsPrincipal user);
        Task<string> GetSenderNameAsync(string senderId);
        Task<string> GetSenderProfilePictureAsync(string senderId);
    }
}
