using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models;

public class Claim
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string ClaimNumber { get; set; } = string.Empty;
    
    [Required]
    public int PolicyId { get; set; }
    
    public int? AssetId { get; set; }
    
    [Required]
    public DateTime ClaimDate { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal ClaimAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal ApprovedAmount { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Submitted"; // Submitted, UnderReview, Approved, Rejected, Paid
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation Properties
    [ForeignKey("PolicyId")]
    public virtual Policy Policy { get; set; } = null!;
    
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
}
