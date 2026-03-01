using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class EnhancedDebugTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EnhancedDebugTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Debug_AssetCreation_StepByStep()
    {
        // Step 1: Login
        Console.WriteLine("Step 1: Logging in...");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@test.com",
            password = "password"
        });
        
        if (!loginResponse.IsSuccessStatusCode)
        {
            var error = await loginResponse.Content.ReadAsStringAsync();
            throw new Exception($"Login failed: {loginResponse.StatusCode} - {error}");
        }
        
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult?.Token);
        Console.WriteLine("✅ Login successful");

        // Step 2: Get all policies
        Console.WriteLine("\nStep 2: Getting policies...");
        var policiesResponse = await _client.GetAsync("/api/policies");
        
        if (!policiesResponse.IsSuccessStatusCode)
        {
            var error = await policiesResponse.Content.ReadAsStringAsync();
            throw new Exception($"Get policies failed: {policiesResponse.StatusCode} - {error}");
        }
        
        var policies = await policiesResponse.Content.ReadFromJsonAsync<List<PolicyDto>>();
        Console.WriteLine($"✅ Got {policies?.Count ?? 0} policies");

        if (policies == null || !policies.Any())
        {
            throw new Exception("No policies found in the database");
        }

        var policyId = policies.First().Id;
        Console.WriteLine($"First policy ID: {policyId}");

        // Step 3: Try to create an asset with that policy ID
        Console.WriteLine("\nStep 3: Creating asset with policy ID {policyId}...");
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

        var createResponse = await _client.PostAsJsonAsync("/api/assets", vehicleAsset);
        
        if (!createResponse.IsSuccessStatusCode)
        {
            var error = await createResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Create failed with status {createResponse.StatusCode}");
            Console.WriteLine($"Error response: {error}");
            
            // Step 4: Check if the policy is valid by getting its details
            Console.WriteLine("\nStep 4: Checking policy details...");
            var policyDetailResponse = await _client.GetAsync($"/api/policies/{policyId}");
            if (policyDetailResponse.IsSuccessStatusCode)
            {
                var policyDetail = await policyDetailResponse.Content.ReadFromJsonAsync<PolicyDetailDto>();
                Console.WriteLine($"✅ Policy exists: {policyDetail?.PolicyNumber} - {policyDetail?.PolicyHolder}");
            }
            else
            {
                var policyError = await policyDetailResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Policy not accessible: {policyDetailResponse.StatusCode} - {policyError}");
            }
            
            throw new Exception($"Create failed with status {createResponse.StatusCode}: {error}");
        }

        var asset = await createResponse.Content.ReadFromJsonAsync<AssetDto>();
        Console.WriteLine($"✅ Asset created successfully with ID: {asset?.Id}");
        
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(asset);
    }

    private class LoginResult
    {
        public string Token { get; set; } = "";
    }

    private class PolicyDto
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = "";
        public string PolicyHolder { get; set; } = "";
    }

    private class PolicyDetailDto
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = "";
        public string PolicyHolder { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
    }

    private class AssetDto
    {
        public int Id { get; set; }
        public string AssetType { get; set; } = "";
        public string Description { get; set; } = "";
        public int PolicyId { get; set; }
    }
}
