using Electro.Core.Models.Identity;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Electro.Core.Entities.Chat
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string SenderId { get; set; } // معرف المرسل
        public string ReceiverId { get; set; } // معرف المستلم
        public string Content { get; set; } // محتوى الرسالة
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public AppUser Sender { get; set; } // علاقة مع الكيان User
        public bool IsRead { get; set; }  // حقل جديد لتخزين حالة القراءة


        [JsonIgnore]
        public Conversation Conversation { get; set; }
        public AppUser Receiver { get; set; }  // يجب أن يكون لديك علاقة مع AppUser
    }
}
