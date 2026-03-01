namespace InsureX.ModernAPI.DTOs;

public class AssetDto
{
    public int Id { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PolicyId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal FinanceValue { get; set; }
    public decimal InsuredValue { get; set; }
    public string Status { get; set; } = string.Empty;
    public Dictionary<string, object> Details { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateAssetDto
{
    public string AssetType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PolicyId { get; set; }
    public decimal FinanceValue { get; set; }
    public decimal InsuredValue { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
}

public class UpdateAssetDto
{
    public string? Description { get; set; }
    public decimal? FinanceValue { get; set; }
    public decimal? InsuredValue { get; set; }
    public string? Status { get; set; }
    public Dictionary<string, object>? Details { get; set; }
}

// DTOs específicos para cada tipo de Asset (opcionais, para validação)
public class VehicleAssetDetails
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? Variant { get; set; }
    public string? VIN { get; set; }
    public int Year { get; set; }
    public string? Registration { get; set; }
}

public class PropertyAssetDetails
{
    public string Address { get; set; } = string.Empty;
    public string? ERFNumber { get; set; }
    public string? SectionalTitle { get; set; }
    public double Size { get; set; }
    public int YearBuilt { get; set; }
}
