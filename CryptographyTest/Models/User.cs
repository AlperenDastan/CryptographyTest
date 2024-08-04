namespace CryptographyTest.Models
{
    public class User : BaseEntity
    {
        public string? UserName { get; set; }  // Use this for login identification
        public string Password { get; set; }
        public string? BadgeNumber { get; set; }
        public string? Email { get; set; }
        public UserRole Role { get; set; }

        // Optionally, if you want a separate UserId property, it can return Id from BaseEntity
        public Guid UserId => this.Id; // Alternatively, you can just use Id directly
    }

    public enum UserRole
    {
        Detective = 0,
        Supervisor = 1,
        Admin = 2,
    }
}
