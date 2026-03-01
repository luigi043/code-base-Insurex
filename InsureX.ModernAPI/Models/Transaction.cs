using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string TransactionNumber { get; set; } = string.Empty;
    
    [Required]
    public int PolicyId { get; set; }
    
    [Required]
    public DateTime TransactionDate { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // Premium, Payment, Refund, Adjustment
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Reference { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey("PolicyId")]
    public virtual Policy Policy { get; set; } = null!;
}
