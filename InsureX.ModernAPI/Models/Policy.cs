namespace InsureX.ModernAPI.Models
{
    public class Policy
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = "";
        public string PolicyHolder { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "";
        public decimal Premium { get; set; }
        public string PolicyType { get; set; } = "";
        public int? PartnerId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        
        // Navigation properties
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
        public ICollection<InsuranceClaim> Claims { get; set; } = new List<InsuranceClaim>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
