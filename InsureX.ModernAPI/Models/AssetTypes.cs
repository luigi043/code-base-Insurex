using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InsureX.ModernAPI.Models
{
    // Base class for all asset types
    public abstract class AssetBase
    {
        public virtual string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

        public virtual void FromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            
            try
            {
                var obj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (obj != null)
                {
                    foreach (var prop in GetType().GetProperties())
                    {
                        if (prop.CanWrite && obj.ContainsKey(prop.Name))
                        {
                            try
                            {
                                var value = obj[prop.Name];
                                if (value != null)
                                {
                                    var targetType = prop.PropertyType;
                                    // Fix the warning by handling null properly
                                    if (value.ToString() != null)
                                    {
                                        var convertedValue = Convert.ChangeType(value.ToString()!, targetType);
                                        prop.SetValue(this, convertedValue);
                                    }
                                }
                            }
                            catch { /* Ignore conversion errors */ }
                        }
                    }
                }
            }
            catch { /* Ignore parsing errors */ }
        }
    }

    // Vehicle Asset
    public class VehicleAsset : AssetBase
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Variant { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string Registration { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int? EngineSize { get; set; }
        public string FuelType { get; set; } = string.Empty;
    }

    // Property Asset
    public class PropertyAsset : AssetBase
    {
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string ErfNumber { get; set; } = string.Empty;
        public string SectionalTitleNumber { get; set; } = string.Empty;
        public double SizeInSquareMeters { get; set; }
        public int YearBuilt { get; set; }
        public string ConstructionType { get; set; } = string.Empty;
    }

    // Watercraft Asset
    public class WatercraftAsset : AssetBase
    {
        public string VesselName { get; set; } = string.Empty;
        public string VesselType { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public double LengthInMeters { get; set; }
        public string HullNumber { get; set; } = string.Empty;
        public string EngineMake { get; set; } = string.Empty;
        public int EngineCount { get; set; }
    }

    // Aviation Asset
    public class AviationAsset : AssetBase
    {
        public string AircraftType { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string TailNumber { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public int EngineCount { get; set; }
        public string EngineMake { get; set; } = string.Empty;
        public int PassengerCapacity { get; set; }
    }

    // Stock Asset
    public class StockAsset : AssetBase
    {
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string Location { get; set; } = string.Empty;
    }

    // Accounts Receivable Asset
    public class AccountsReceivableAsset : AssetBase
    {
        public string DebtorName { get; set; } = string.Empty;
        public string DebtorReference { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int InvoiceCount { get; set; }
    }

    // Machinery Asset
    public class MachineryAsset : AssetBase
    {
        public string EquipmentType { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Capacity { get; set; } = string.Empty;
    }

    // Plant & Equipment Asset
    public class PlantEquipmentAsset : AssetBase
    {
        public string EquipmentType { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Capacity { get; set; } = string.Empty;
    }

    // Business Interruption Asset
    public class BusinessInterruptionAsset : AssetBase
    {
        public decimal AnnualRevenue { get; set; }
        public decimal GrossProfit { get; set; }
        public int IndemnityPeriodMonths { get; set; }
        public string BusinessType { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
    }

    // Keyman Insurance Asset
    public class KeymanAsset : AssetBase
    {
        public string PersonName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal AnnualSalary { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IdNumber { get; set; } = string.Empty;
    }

    // Electronic Equipment Asset
    public class ElectronicEquipmentAsset : AssetBase
    {
        public string EquipmentType { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public int Year { get; set; }
        public string WarrantyExpiryDate { get; set; } = string.Empty;
    }

    // Factory to create the appropriate asset type
    public static class AssetFactory
    {
        public static AssetBase CreateAsset(string assetType, string? jsonData = null)
        {
            AssetBase asset = assetType.ToLower() switch
            {
                "vehicle" => new VehicleAsset(),
                "property" => new PropertyAsset(),
                "watercraft" => new WatercraftAsset(),
                "aviation" => new AviationAsset(),
                "stock" => new StockAsset(),
                "accountsreceivable" => new AccountsReceivableAsset(),
                "machinery" => new MachineryAsset(),
                "plantequipment" => new PlantEquipmentAsset(),
                "businessinterruption" => new BusinessInterruptionAsset(),
                "keyman" => new KeymanAsset(),
                "electronicequipment" => new ElectronicEquipmentAsset(),
                _ => throw new ArgumentException($"Unknown asset type: {assetType}")
            };

            if (!string.IsNullOrEmpty(jsonData))
            {
                asset.FromJson(jsonData);
            }

            return asset;
        }

        public static string GetAssetType(AssetBase asset)
        {
            return asset switch
            {
                VehicleAsset _ => "Vehicle",
                PropertyAsset _ => "Property",
                WatercraftAsset _ => "Watercraft",
                AviationAsset _ => "Aviation",
                StockAsset _ => "Stock",
                AccountsReceivableAsset _ => "AccountsReceivable",
                MachineryAsset _ => "Machinery",
                PlantEquipmentAsset _ => "PlantEquipment",
                BusinessInterruptionAsset _ => "BusinessInterruption",
                KeymanAsset _ => "Keyman",
                ElectronicEquipmentAsset _ => "ElectronicEquipment",
                _ => "Unknown"
            };
        }
    }
}
