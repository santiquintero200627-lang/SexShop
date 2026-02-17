using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SexShop.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new();
    }
}
