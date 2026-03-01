using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models
{
    public class InsuranceClaim
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ClaimNumber { get; set; } = string.Empty;
        
        [Required]
        public int PolicyId { get; set; }
        
        public int? AssetId { get; set; }
        
        [Required]
        public DateTime ClaimDate { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ClaimAmount { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ApprovedAmount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("PolicyId")]
        public virtual Policy? Policy { get; set; }
        
        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }
    }
}
