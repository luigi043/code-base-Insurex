using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models
{
    public class RecurringBilling
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int PolicyId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Frequency { get; set; } = string.Empty; // Monthly, Quarterly, Annually
        
        [Required]
        public DateTime NextBillingDate { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int? LastInvoiceId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("PolicyId")]
        public virtual Policy? Policy { get; set; }
        
        [ForeignKey("LastInvoiceId")]
        public virtual Invoice? LastInvoice { get; set; }
    }
}
