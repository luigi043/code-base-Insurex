using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace InsureX.IntegrationTests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task Auth_Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginRequest = new
        {
            email = "admin@insurex.com",
            password = "password" // Use the actual password from seed
        };
        
        var json = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/login", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
        Assert.True(tokenResponse.TryGetProperty("token", out var token));
        Assert.False(string.IsNullOrWhiteSpace(token.GetString()));
    }

    [Fact]
    public async Task Policies_GetAll_ReturnsPolicies()
    {
        // Act
        var response = await _client.GetAsync("/api/policies");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var policies = await response.Content.ReadFromJsonAsync<List<Policy>>();
        Assert.NotNull(policies);
        Assert.True(policies.Count > 0);
    }

    [Fact]
    public async Task Assets_GetByType_ReturnsCorrectAssets()
    {
        // Act
        var response = await _client.GetAsync("/api/assets?type=Vehicle");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assets = await response.Content.ReadFromJsonAsync<List<Asset>>();
        Assert.NotNull(assets);
        Assert.All(assets, a => Assert.Equal("Vehicle", a.AssetType));
    }

    [Fact]
    public async Task Claims_Create_WithValidData_ReturnsCreated()
    {
        // Arrange - First login to get token
        var loginResponse = await LoginAndGetToken();
        var token = loginResponse.token;
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var newClaim = new
        {
            policyId = 1,
            claimNumber = $"CLM-TEST-{DateTime.Now.Ticks}",
            claimDate = DateTime.UtcNow,
            description = "Test claim from integration test",
            claimAmount = 15000.00m,
            status = "Pending"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/claims", newClaim);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task<dynamic> LoginAndGetToken()
    {
        var loginRequest = new
        {
            email = "admin@insurex.com",
            password = "password"
        };
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        return new { token = content.GetProperty("token").GetString() };
    }
}

// Simple DTOs for testing
public class Policy
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class Asset
{
    public int Id { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public decimal InsuredValue { get; set; }
}