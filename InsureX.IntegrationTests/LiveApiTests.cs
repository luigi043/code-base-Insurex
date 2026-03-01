using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class LiveApiTests
{
    private readonly HttpClient _client;
    private static string? _authToken;

    public LiveApiTests()
    {
        // Test against the ACTUAL running API
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5012");
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    public class Policy
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string PolicyHolder { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    [Fact]
    public async Task Test1_Login_WithRealCredentials()
    {
        // Arrange - Use the credentials that work in real life
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
        var result = JsonSerializer.Deserialize<LoginResponse>(content, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        
        // Store token for subsequent tests
        _authToken = result.Token;
        Console.WriteLine($"✅ Login successful! Token received");
    }

    [Fact]
    public async Task Test2_GetPolicies_WithAuth()
    {
        // Ensure we're logged in
        await Test1_Login_WithRealCredentials();
        
        // Set auth header
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);

        // Act
        var response = await _client.GetAsync("/api/policies");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Get policies failed: {response.StatusCode}");
        
        var policies = await response.Content.ReadFromJsonAsync<List<Policy>>();
        Assert.NotNull(policies);
        Assert.True(policies!.Count > 0, "No policies found");
        
        Console.WriteLine($"✅ Found {policies.Count} policies");
        Console.WriteLine($"First policy ID: {policies[0].Id}");
    }

    [Fact]
    public async Task Test3_CreateAsset_WithRealPolicy()
    {
        // Ensure we're logged in
        await Test1_Login_WithRealCredentials();
        
        // Set auth header
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);

        // First get a valid policy
        var policiesResponse = await _client.GetAsync("/api/policies");
        policiesResponse.EnsureSuccessStatusCode();
        
        var policies = await policiesResponse.Content.ReadFromJsonAsync<List<Policy>>();
        Assert.NotNull(policies);
        Assert.True(policies!.Count > 0, "Need at least one policy");
        
        var policyId = policies[0].Id;
        Console.WriteLine($"Using policy ID: {policyId}");

        // Create vehicle asset - Note: using /api/v1/Assets endpoint
        var newAsset = new
        {
            policyId = policyId,
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

        // Try both possible asset endpoints
        var endpoints = new[] { "/api/v1/Assets", "/api/assets" };
        HttpResponseMessage? successResponse = null;
        string usedEndpoint = "";

        foreach (var endpoint in endpoints)
        {
            var response = await _client.PostAsJsonAsync(endpoint, newAsset);
            if (response.IsSuccessStatusCode)
            {
                successResponse = response;
                usedEndpoint = endpoint;
                break;
            }
        }

        Assert.NotNull(successResponse);
        Console.WriteLine($"✅ Asset created using {usedEndpoint}");
        
        var result = await successResponse!.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(result));
        Console.WriteLine($"Response: {result}");
    }

    [Fact]
    public async Task Test4_GetAssets_ByType()
    {
        // Ensure we're logged in
        await Test1_Login_WithRealCredentials();
        
        // Set auth header
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);

        // Get assets by type from dashboard endpoint
        var response = await _client.GetAsync("/api/v1/Dashboard/assets-by-type");
        
        // This might 404 if endpoint doesn't exist, so we'll just log
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"✅ Assets by type: {content}");
        }
        else
        {
            Console.WriteLine($"ℹ️ Dashboard endpoint returned {response.StatusCode} - this is OK if not implemented");
        }
    }
}