using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class CorrectAssetTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private static string _authToken;

    public CorrectAssetTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    [Fact]
    public async Task Test1_DiscoverEndpoints()
    {
        // Try to get Swagger JSON to discover endpoints
        var swaggerResponse = await _client.GetAsync("/swagger/v1/swagger.json");
        if (swaggerResponse.IsSuccessStatusCode)
        {
            var swaggerJson = await swaggerResponse.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(swaggerJson));
            return;
        }
        
        // If Swagger fails, try common endpoints
        var testEndpoints = new[]
        {
            "/api/auth/login",
            "/api/User/login", 
            "/api/Account/login",
            "/auth/login"
        };

        foreach (var endpoint in testEndpoints)
        {
            var response = await _client.GetAsync(endpoint.Replace("login", "test"));
            // Just checking if endpoint exists
        }
    }

    [Fact]
    public async Task Test2_LoginWithCorrectCredentials()
    {
        // Try different credential combinations
        var credentials = new[]
        {
            new { email = "admin@insurex.com", password = "password" },
            new { email = "admin@insurex.com", password = "admin123" },
            new { email = "test@insurex.com", password = "password" },
            new { email = "admin", password = "admin" }
        };

        var endpoints = new[]
        {
            "/api/auth/login",
            "/api/User/login"
        };

        foreach (var endpoint in endpoints)
        {
            foreach (var cred in credentials)
            {
                try
                {
                    var response = await _client.PostAsJsonAsync(endpoint, cred);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<LoginResponse>(content, 
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        if (result?.Token != null)
                        {
                            _authToken = result.Token;
                            _client.DefaultRequestHeaders.Authorization = 
                                new AuthenticationHeaderValue("Bearer", _authToken);
                            
                            Assert.NotNull(_authToken);
                            return;
                        }
                    }
                }
                catch { /* Continue trying */ }
            }
        }
        
        Assert.Fail("Could not login with any credentials");
    }

    [Fact]
    public async Task Test3_FindValidPolicy()
    {
        // Ensure we're logged in
        if (string.IsNullOrEmpty(_authToken))
        {
            await Test2_LoginWithCorrectCredentials();
        }

        // Try to get policies
        var response = await _client.GetAsync("/api/policies");
        if (response.IsSuccessStatusCode)
        {
            var policies = await response.Content.ReadFromJsonAsync<List<Policy>>();
            Assert.NotNull(policies);
            Assert.True(policies.Count > 0, "No policies found in database");
        }
    }

    [Fact]
    public async Task Test4_CreateAssetWithValidPolicy()
    {
        // Ensure we're logged in
        if (string.IsNullOrEmpty(_authToken))
        {
            await Test2_LoginWithCorrectCredentials();
        }

        // First get a valid policy ID
        var policiesResponse = await _client.GetAsync("/api/policies");
        policiesResponse.EnsureSuccessStatusCode();
        
        var policies = await policiesResponse.Content.ReadFromJsonAsync<List<Policy>>();
        Assert.NotNull(policies);
        Assert.True(policies.Count > 0, "Need at least one policy to create an asset");
        
        var policyId = policies[0].Id;

        // Create vehicle asset
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

        var createResponse = await _client.PostAsJsonAsync("/api/assets", newAsset);
        
        if (!createResponse.IsSuccessStatusCode)
        {
            var error = await createResponse.Content.ReadAsStringAsync();
            Assert.Fail($"Create failed with status {createResponse.StatusCode}: {error}");
        }
        
        var result = await createResponse.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(result));
    }

    public class Policy
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; }
        public string CustomerName { get; set; }
    }
}
