using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public Product Product { get; set; }
    }
}
