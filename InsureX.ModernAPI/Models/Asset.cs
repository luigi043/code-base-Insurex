using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using InsureX.ModernAPI.Helpers;

namespace InsureX.ModernAPI.Models
{
    public class Asset
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string AssetType { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public int PolicyId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinanceValue { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuredValue { get; set; }
        
        [StringLength(50)]
        public string? Status { get; set; } = "Active";
        
        [Required]
        public string JsonData { get; set; } = "{}";
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }
        
        // Navigation properties
        [ForeignKey("PolicyId")]
        [JsonIgnore]
        public virtual Policy? Policy { get; set; }

        // Helper method to get typed asset
        [JsonIgnore]
        public AssetBase? TypedAsset
        {
            get
            {
                try
                {
                    return AssetFactory.CreateAsset(AssetType, JsonData);
                }
                catch
                {
                    return null;
                }
            }
        }

        // Helper method to update JsonData from typed asset
        public void UpdateJsonData(AssetBase asset)
        {
            JsonData = asset.ToJson();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
