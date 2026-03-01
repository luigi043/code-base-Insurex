using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models;

public class Policy
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PolicyNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string PolicyHolder { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Active";
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Premium { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PolicyType { get; set; } = string.Empty;
    
    public int? PartnerId { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; } = false;
}
