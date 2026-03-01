using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TransactionNumber { get; set; } = string.Empty;
        
        [Required]
        public int PolicyId { get; set; }
        
        [Required]
        public DateTime TransactionDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? Reference { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("PolicyId")]
        public virtual Policy? Policy { get; set; }
    }
}
