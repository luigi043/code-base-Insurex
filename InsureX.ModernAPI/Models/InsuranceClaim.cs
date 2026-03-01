namespace InsureX.ModernAPI.Models
{
    public class InsuranceClaim
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = "";
        public int PolicyId { get; set; }
        public int? AssetId { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Description { get; set; } = "";
        public decimal ClaimAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string Status { get; set; } = "";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public Policy Policy { get; set; } = null!;
        public Asset? Asset { get; set; }
    }
}
