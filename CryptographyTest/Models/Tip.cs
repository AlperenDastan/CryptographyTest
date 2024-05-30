namespace CryptographyTest.Models
{
    public class Tip : BaseEntity
    {
        public string? Description { get; set; }
        public ContactPerson? ContactPerson { get; set; }
        public DateTime LogDate { get; set; }
        public TipLevel Level { get; set; }

    }

    public enum TipLevel
    {
        Public = 0,
        Classified = 1,
    }
}
