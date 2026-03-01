using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class AssetEndpointTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;

    public AssetEndpointTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    [Fact]
    public async Task Step1_Login_And_GetToken()
    {
        // Arrange
        var loginRequest = new
        {
            email = "admin@insurex.com",
            password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Login failed: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        
        // Store token for subsequent tests
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Token);
    }

    [Fact]
    public async Task Step2_Get_All_Assets()
    {
        // First login to get token
        await Step1_Login_And_GetToken();
        
        // Act - Get all assets
        var response = await _client.GetAsync("/api/assets");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Get assets failed: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(content));
    }

    [Fact]
    public async Task Step3_Create_New_Asset()
    {
        // First login to get token
        await Step1_Login_And_GetToken();
        
        // Arrange - Create a vehicle asset
        var newAsset = new
        {
            policyId = 1,
            assetType = "Vehicle",
            assetData = new
            {
                Make = "Tesla",
                Model = "Model 3",
                Year = 2023,
                VIN = "5YJ3E1EAXPF123456",
                Registration = "ABC-123",
                FinanceValue = 45000.00m,
                InsuredValue = 50000.00m
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/assets", newAsset);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Create asset failed: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(content));
    }
}
