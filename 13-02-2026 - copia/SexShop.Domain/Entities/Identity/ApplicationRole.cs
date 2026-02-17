using Microsoft.AspNetCore.Identity;

namespace SexShop.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; } = string.Empty;
    }
}
