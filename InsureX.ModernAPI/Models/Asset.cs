using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace InsureX.ModernAPI.Models;

public class Asset
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string AssetType { get; set; } = string.Empty; // Vehicle, Property, Watercraft, etc.
    
    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public int PolicyId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal FinanceValue { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal InsuredValue { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; } = "Active";
    
    [Required]
    public string JsonData { get; set; } = "{}";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    
    // Navigation Properties
    [ForeignKey("PolicyId")]
    public virtual Policy Policy { get; set; } = null!;
    
    // Helper methods para acessar JsonData
    public T? GetJsonValue<T>(string key)
    {
        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonData);
            if (data != null && data.ContainsKey(key))
            {
                var element = (JsonElement)data[key];
                return JsonSerializer.Deserialize<T>(element.GetRawText());
            }
        }
        catch { }
        return default;
    }
    
    public string? GetJsonValue(string key)
    {
        return GetJsonValue<string?>(key);
    }
    
    public void SetJsonValue(string key, object? value)
    {
        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonData) ?? new();
            if (value == null)
                data.Remove(key);
            else
                data[key] = value;
            
            JsonData = JsonSerializer.Serialize(data);
        }
        catch { }
    }
}
