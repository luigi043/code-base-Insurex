using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models
{
    public class InvoiceItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int InvoiceId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public int Quantity { get; set; } = 1;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ItemType { get; set; } = string.Empty; // Premium, Fee, Tax, Adjustment
        
        public int? ReferenceId { get; set; }
        
        [StringLength(50)]
        public string? ReferenceType { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        [ForeignKey("InvoiceId")]
        public virtual Invoice? Invoice { get; set; }
    }
}
