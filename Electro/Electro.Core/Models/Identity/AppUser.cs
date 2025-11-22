using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Models.Identity
{
    public class AppUser :IdentityUser
    {
        public string FullName { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string Role {  get; set; }
        public UserStatus Status { get; set; }


    }
    public enum UserStatus
    {   
        Active,
        Inactive,
        Banned,
        Rejected,
        Pending,
        Deleted
    }

}
