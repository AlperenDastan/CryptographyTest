using Microsoft.AspNetCore.Identity;

namespace CryptographyTest.Models
{
    public class User : IdentityUser<Guid>  // Inherits from IdentityUser with Guid as the primary key
    {
        public string? BadgeNumber { get; set; } // Additional property specific to your application
        public UserRole Role { get; set; }       // Additional property for role management
    }

    public enum UserRole
    {
        Detective = 0,
        Supervisor = 1,
        Admin = 2,
    }
}

