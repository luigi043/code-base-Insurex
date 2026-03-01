using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;
        
        [Required]
        public int PolicyId { get; set; }
        
        [Required]
        public DateTime InvoiceDate { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Draft";
        
        [StringLength(50)]
        public string? PaymentMethod { get; set; }
        
        public DateTime? PaymentDate { get; set; }
        
        public string? Notes { get; set; }
        
        [StringLength(500)]
        public string? PdfUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public int? CreatedBy { get; set; }
        
        // Navigation properties
        [ForeignKey("PolicyId")]
        public virtual Policy? Policy { get; set; }
        
        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }
        
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
