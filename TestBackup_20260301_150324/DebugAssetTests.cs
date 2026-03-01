using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class DebugAssetTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DebugAssetTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Debug_AssetCreation()
    {
        // Login
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@test.com",
            password = "password"
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult?.Token);

        // Get all policies
        var policiesResponse = await _client.GetAsync("/api/policies");
        policiesResponse.EnsureSuccessStatusCode();
        var policies = await policiesResponse.Content.ReadFromJsonAsync<List<PolicyDto>>();
        
        Assert.NotNull(policies);
        Assert.NotEmpty(policies);
        
        var policyId = policies.First().Id;
        
        // Try to create an asset
        var vehicleAsset = new
        {
            policyId = policyId,
            assetType = "Vehicle",
            description = "Debug Vehicle",
            financeValue = 25000.00M,
            insuredValue = 28000.00M,
            status = "Active",
            assetData = new
            {
                make = "Toyota",
                model = "Camry",
                year = 2023,
                vin = "4T1BF1FK8PU123456"
            }
        };

        var response = await _client.PostAsJsonAsync("/api/assets", vehicleAsset);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Create failed with status {response.StatusCode}: {error}");
        }

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
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
}
