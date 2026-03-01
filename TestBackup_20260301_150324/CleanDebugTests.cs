using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class CleanDebugTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CleanDebugTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Debug_AssetCreation_StepByStep()
    {
        // Step 1: Login
        System.Console.WriteLine("Step 1: Logging in...");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@test.com",
            password = "password"
        });
        
        if (!loginResponse.IsSuccessStatusCode)
        {
            var error = await loginResponse.Content.ReadAsStringAsync();
            System.Console.WriteLine($"Login failed: {loginResponse.StatusCode} - {error}");
            throw new Exception($"Login failed: {loginResponse.StatusCode} - {error}");
        }
        
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult?.Token);
        System.Console.WriteLine("✅ Login successful");

        // Step 2: Get all policies
        System.Console.WriteLine("\nStep 2: Getting policies...");
        var policiesResponse = await _client.GetAsync("/api/policies");
        
        if (!policiesResponse.IsSuccessStatusCode)
        {
            var error = await policiesResponse.Content.ReadAsStringAsync();
            System.Console.WriteLine($"Get policies failed: {policiesResponse.StatusCode} - {error}");
            throw new Exception($"Get policies failed: {policiesResponse.StatusCode} - {error}");
        }
        
        var policies = await policiesResponse.Content.ReadFromJsonAsync<List<PolicyDto>>();
        System.Console.WriteLine($"✅ Got {policies?.Count ?? 0} policies");

        if (policies == null || !policies.Any())
        {
            System.Console.WriteLine("No policies found, creating one...");
            
            var newPolicy = new
            {
                policyNumber = $"POL-DEBUG-{DateTime.Now.Ticks}",
                policyHolder = "Debug Customer",
                email = "debug@test.com",
                phone = "123-456-7890",
                startDate = DateTime.UtcNow,
                endDate = DateTime.UtcNow.AddYears(1),
                status = "Active",
                premium = 1000.00M,
                policyType = "Comprehensive"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/policies", newPolicy);
            if (!createResponse.IsSuccessStatusCode)
            {
                var error = await createResponse.Content.ReadAsStringAsync();
                System.Console.WriteLine($"Create policy failed: {createResponse.StatusCode} - {error}");
                throw new Exception($"Create policy failed: {createResponse.StatusCode} - {error}");
            }

            var createdPolicy = await createResponse.Content.ReadFromJsonAsync<PolicyDto>();
            policies = new List<PolicyDto> { createdPolicy! };
            System.Console.WriteLine($"✅ Created new policy with ID: {createdPolicy?.Id}");
        }

        var policyId = policies!.First().Id;
        System.Console.WriteLine($"Using policy ID: {policyId}");

        // Step 3: Verify policy exists
        System.Console.WriteLine("\nStep 3: Verifying policy exists...");
        var policyDetailResponse = await _client.GetAsync($"/api/policies/{policyId}");
        if (policyDetailResponse.IsSuccessStatusCode)
        {
            var policyDetail = await policyDetailResponse.Content.ReadFromJsonAsync<PolicyDetailDto>();
            System.Console.WriteLine($"✅ Policy verified: {policyDetail?.PolicyNumber} - {policyDetail?.PolicyHolder}");
        }
        else
        {
            var policyError = await policyDetailResponse.Content.ReadAsStringAsync();
            System.Console.WriteLine($"❌ Policy not accessible: {policyDetailResponse.StatusCode} - {policyError}");
            throw new Exception($"Policy {policyId} not accessible");
        }

        // Step 4: Create asset with correct format and versioned endpoint
        System.Console.WriteLine("\nStep 4: Creating asset...");
        var vehicleAsset = new
        {
            assetType = "Vehicle",
            description = "Debug Vehicle",
            policyId = policyId,
            financeValue = 25000.00M,
            insuredValue = 28000.00M,
            details = new Dictionary<string, object>
            {
                ["make"] = "Toyota",
                ["model"] = "Camry",
                ["year"] = 2023,
                ["vin"] = "4T1BF1FK8PU123456",
                ["registration"] = "ABC-123"
            }
        };

        // FIXED: Use versioned endpoint /api/v1/assets
        var createAssetResponse = await _client.PostAsJsonAsync("/api/v1/assets", vehicleAsset);
        
        if (!createAssetResponse.IsSuccessStatusCode)
        {
            var error = await createAssetResponse.Content.ReadAsStringAsync();
            System.Console.WriteLine($"❌ Create asset failed with status {createAssetResponse.StatusCode}");
            System.Console.WriteLine($"Error response: {error}");
            
            // If 400 Bad Request, the error message will tell us what's wrong
            if (createAssetResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                System.Console.WriteLine("⚠️ Bad Request - check the asset object format matches the API expectation");
            }
            
            throw new Exception($"Create asset failed with status {createAssetResponse.StatusCode}: {error}");
        }

        var asset = await createAssetResponse.Content.ReadFromJsonAsync<AssetDto>();
        System.Console.WriteLine($"✅ Asset created successfully with ID: {asset?.Id}");
        
        Assert.Equal(HttpStatusCode.Created, createAssetResponse.StatusCode);
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
    }

    private class AssetDto
    {
        public int Id { get; set; }
        public string AssetType { get; set; } = "";
        public string Description { get; set; } = "";
        public int PolicyId { get; set; }
    }
}