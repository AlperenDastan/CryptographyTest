namespace CryptographyTest.Models
{
    public class Case : BaseEntity
    {
        public string? Name { get; set; }
        public string? SerialNumber { get; set; }
        public List<string?>? Notes { get; set; }
        public string? Description { get; set; }
        public List<Tip>? Tips { get; set; }

        // Foreign key for Detective
        public Guid DetectiveId { get; set; }  // Add this property
        public User? Detective { get; set; }

        // Foreign key for Supervisor
        public Guid SupervisorId { get; set; }  // Add this property
        public User? Supervisor { get; set; }

        public CaseStatus Status { get; set; }
    }

    public enum CaseStatus
    {
        Open = 0,
        Close = 1,
        Active = 2,
        Cold = 3,
    }
}
