using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Chat
{
    public class ConversationDTO
    {
        public int Id { get; set; }
        public string SenderId { get; set; } // معرف المرسل
        public string ReceiverId { get; set; } // معرف المستقبل
        public string UserName { get; set; } // اسم المرسل
        public string UserImage { get; set; } // صورة المرسل
        public string LastMessageContent { get; set; } // محتوى الرسالة الأخيرة
        public int UnreadMessagesCount { get; set; } // محتوى الرسالة الأخيرة
        public DateTime? LastMessageTimestamp { get; set; } // توقيت الرسالة الأخيرة
    }
}
