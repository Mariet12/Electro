using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos
{
    public class CommunicationMethodCreateDto
    {
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UrlLocation { get; set; } = null!;

    }

    public class CommunicationMethodReadDto : CommunicationMethodCreateDto
    {
        public int Id { get; set; }
    }
}
