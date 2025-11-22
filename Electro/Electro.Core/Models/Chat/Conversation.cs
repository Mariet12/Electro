using Electro.Core.Models.Identity;

namespace Electro.Core.Entities.Chat
{
    public class Conversation
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Message> Messages { get; set; }

        
        public virtual AppUser Sender { get; set; }
        public virtual AppUser Receiver { get; set; }
    }
}
