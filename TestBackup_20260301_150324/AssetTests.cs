using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class AssetTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private string? _token;

    public AssetTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task LoginAsync()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@test.com",
            password = "password"
        });

        loginResponse.EnsureSuccessStatusCode();
        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
        _token = result?.Token ?? "";
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    private async Task<int> GetFirstPolicyIdAsync()
    {
        var response = await _client.GetAsync("/api/policies");
        response.EnsureSuccessStatusCode();
        
        var policies = await response.Content.ReadFromJsonAsync<List<PolicyDto>>();
        Assert.NotNull(policies);
        Assert.NotEmpty(policies);
        
        return policies.First().Id;
    }

    [Fact]
    public async Task GetAllAssets_ReturnsSuccess()
    {
        await LoginAsync();
        
        // Act
        var response = await _client.GetAsync("/api/assets");

        // Assert - Accept either OK or NotFound (if no assets exist)
        Assert.True(response.StatusCode == HttpStatusCode.OK || 
                    response.StatusCode == HttpStatusCode.NotFound,
                    $"Expected OK or NotFound but got {response.StatusCode}");
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var assets = await response.Content.ReadFromJsonAsync<List<AssetDto>>();
            Assert.NotNull(assets);
        }
    }

    [Fact]
    public async Task CreateVehicleAsset_ReturnsSuccess()
    {
        await LoginAsync();
        var policyId = await GetFirstPolicyIdAsync();

        // Arrange
        var vehicleAsset = new
        {
            policyId = policyId,
            assetType = "Vehicle",
            description = "Test Vehicle",
            financeValue = 25000.00M,
            insuredValue = 28000.00M,
            status = "Active",
            assetData = new
            {
                make = "Toyota",
                model = "Camry",
                year = 2023,
                vin = "4T1BF1FK8PU123456",
                registration = "ABC-123"
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/assets", vehicleAsset);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Create failed with status {response.StatusCode}: {error}");
        }

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var asset = await response.Content.ReadFromJsonAsync<AssetDto>();
        Assert.NotNull(asset);
        Assert.Equal("Vehicle", asset.AssetType);
        Assert.Equal(vehicleAsset.description, asset.Description);
    }

    [Fact]
    public async Task CreatePropertyAsset_ReturnsSuccess()
    {
        await LoginAsync();
        var policyId = await GetFirstPolicyIdAsync();

        // Arrange
        var propertyAsset = new
        {
            policyId = policyId,
            assetType = "Property",
            description = "Test Property",
            financeValue = 350000.00M,
            insuredValue = 375000.00M,
            status = "Active",
            assetData = new
            {
                address = "123 Main St",
                city = "Springfield",
                state = "IL",
                zipCode = "62701",
                propertyType = "Commercial",
                squareFeet = 2500
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/assets", propertyAsset);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Create failed with status {response.StatusCode}: {error}");
        }

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var asset = await response.Content.ReadFromJsonAsync<AssetDto>();
        Assert.NotNull(asset);
        Assert.Equal("Property", asset.AssetType);
    }

    [Fact]
    public async Task GetAssetById_ReturnsAsset()
    {
        await LoginAsync();
        var policyId = await GetFirstPolicyIdAsync();

        // First create an asset
        var vehicleAsset = new
        {
            policyId = policyId,
            assetType = "Vehicle",
            description = "Test Asset for Get",
            financeValue = 10000.00M,
            insuredValue = 12000.00M,
            status = "Active",
            assetData = new { make = "Honda", model = "Civic", year = 2022 }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/assets", vehicleAsset);
        
        if (!createResponse.IsSuccessStatusCode)
        {
            var error = await createResponse.Content.ReadAsStringAsync();
            throw new Exception($"Create failed with status {createResponse.StatusCode}: {error}");
        }

        var created = await createResponse.Content.ReadFromJsonAsync<AssetDto>();
        Assert.NotNull(created);

        // Act
        var response = await _client.GetAsync($"/api/assets/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var asset = await response.Content.ReadFromJsonAsync<AssetDto>();
        Assert.NotNull(asset);
        Assert.Equal(created.Id, asset.Id);
    }

    private class LoginResult
    {
        public string Token { get; set; } = "";
    }

    private class PolicyDto
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = "";
    }

    private class AssetDto
    {
        public int Id { get; set; }
        public string AssetType { get; set; } = "";
        public string Description { get; set; } = "";
        public int PolicyId { get; set; }
        public decimal FinanceValue { get; set; }
        public decimal InsuredValue { get; set; }
        public string Status { get; set; } = "";
    }
}
