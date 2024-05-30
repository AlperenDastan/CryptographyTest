namespace CryptographyTest.Models
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public string Password { get; set; }
        public string? BadgeNumber { get; set; }
        public string? Email { get; set; }
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Detective = 0,
        Supervisor = 1,
        Admin = 2,
    }
}
