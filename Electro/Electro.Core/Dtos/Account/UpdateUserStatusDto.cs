using Electro.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Account
{
    public class UpdateUserStatusDto
    {
        [Required]
        public UserStatus Status { get; set; }
    }
}
