using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InsureX.ModernAPI.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static T? Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;
            
            try
            {
                return JsonSerializer.Deserialize<T>(json, _options);
            }
            catch
            {
                return default;
            }
        }

        public static string Serialize<T>(T obj)
        {
            if (obj == null)
                return string.Empty;
            
            try
            {
                return JsonSerializer.Serialize(obj, _options);
            }
            catch
            {
                return string.Empty;
            }
        }

        // Helper methods for asset types
        public static T? GetJsonValue<T>(string jsonData, string propertyName)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonData);
                if (doc.RootElement.TryGetProperty(propertyName, out var element))
                {
                    return JsonSerializer.Deserialize<T>(element.GetRawText());
                }
            }
            catch
            {
                // Ignore errors
            }
            return default;
        }

        public static string SetJsonValue(string jsonData, string propertyName, object value)
        {
            try
            {
                var obj = string.IsNullOrEmpty(jsonData) 
                    ? new Dictionary<string, object>() 
                    : JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData) ?? new();
                
                obj[propertyName] = value;
                return JsonSerializer.Serialize(obj);
            }
            catch
            {
                return jsonData;
            }
        }
    }
}
