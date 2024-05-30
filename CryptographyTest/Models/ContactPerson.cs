namespace CryptographyTest.Models
{
    public class ContactPerson : BaseEntity
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Notes { get; set; }
    }
}
