using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Chat
{
    public class CreateConversationDTO
    {
        public string SenderId { get; set; } // معرف المرسل
        public string ReceiverId { get; set; } // معرف المستقبل
    }
}
