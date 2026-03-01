using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentNumber { get; set; } = string.Empty;
        
        [Required]
        public int InvoiceId { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // Credit Card, Bank Transfer, Cash, Cheque
        
        [StringLength(100)]
        public string? Reference { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
        
        [StringLength(100)]
        public string? TransactionId { get; set; }
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public int? CreatedBy { get; set; }
        
        // Navigation properties
        [ForeignKey("InvoiceId")]
        public virtual Invoice? Invoice { get; set; }
        
        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }
    }
}
