namespace InsureX.ModernAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string TransactionNumber { get; set; } = "";
        public int PolicyId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } = "";
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public Policy Policy { get; set; } = null!;
    }
}
