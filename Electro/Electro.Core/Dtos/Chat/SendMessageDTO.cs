using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Chat
{
    public class SendMessageDTO
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }

    }
}
