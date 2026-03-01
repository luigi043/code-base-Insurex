using System.ComponentModel.DataAnnotations.Schema;

namespace InsureX.ModernAPI.Models;

// Estas classes são apenas para typed access, não são mapeadas pelo EF
public class VehicleAsset : Asset
{
    [NotMapped]
    public string? Make 
    { 
        get => GetJsonValue("Make");
        set => SetJsonValue("Make", value);
    }
    
    [NotMapped]
    public string? Model 
    { 
        get => GetJsonValue("Model");
        set => SetJsonValue("Model", value);
    }
    
    [NotMapped]
    public string? Variant 
    { 
        get => GetJsonValue("Variant");
        set => SetJsonValue("Variant", value);
    }
    
    [NotMapped]
    public string? VIN 
    { 
        get => GetJsonValue("VIN");
        set => SetJsonValue("VIN", value);
    }
    
    [NotMapped]
    public int? Year 
    { 
        get => GetJsonValue<int?>("Year");
        set => SetJsonValue("Year", value);
    }
    
    [NotMapped]
    public string? Registration 
    { 
        get => GetJsonValue("Registration");
        set => SetJsonValue("Registration", value);
    }
}

public class PropertyAsset : Asset
{
    [NotMapped]
    public string? Address 
    { 
        get => GetJsonValue("Address");
        set => SetJsonValue("Address", value);
    }
    
    [NotMapped]
    public string? ERFNumber 
    { 
        get => GetJsonValue("ERFNumber");
        set => SetJsonValue("ERFNumber", value);
    }
    
    [NotMapped]
    public string? SectionalTitle 
    { 
        get => GetJsonValue("SectionalTitle");
        set => SetJsonValue("SectionalTitle", value);
    }
    
    [NotMapped]
    public double? Size 
    { 
        get => GetJsonValue<double?>("Size");
        set => SetJsonValue("Size", value);
    }
}
