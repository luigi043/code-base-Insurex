namespace InsureX.ModernAPI.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string AssetType { get; set; } = "";
        public string Description { get; set; } = "";
        public int PolicyId { get; set; }
        public decimal FinanceValue { get; set; }
        public decimal InsuredValue { get; set; }
        public string Status { get; set; } = "";
        public string JsonData { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        
        // Navigation property
        public Policy Policy { get; set; } = null!;
    }
}
