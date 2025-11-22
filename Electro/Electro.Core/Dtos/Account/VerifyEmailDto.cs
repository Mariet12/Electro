using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Account
{
    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }

}
