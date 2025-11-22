using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Account
{
    public class VerifyOtp
    {
        [Required]
        public string Email { get; set; }

        public string Otp { get; set; }
    }
}
