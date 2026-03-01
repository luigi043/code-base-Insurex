using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models
{
    public class Policy
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PolicyNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string PolicyHolder { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Premium { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PolicyType { get; set; } = string.Empty;
        
        public int? PartnerId { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }
        
        // Navigation properties - FIXED: Changed from Claim to InsuranceClaim
        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
        public virtual ICollection<InsuranceClaim> Claims { get; set; } = new List<InsuranceClaim>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
