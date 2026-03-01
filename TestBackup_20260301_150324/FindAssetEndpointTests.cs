using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace InsureX.IntegrationTests;

// FIXED: Add the Program type argument
public class FindAssetEndpointTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private string _token;

    public FindAssetEndpointTests(TestWebApplicationFactory<Program> factory)  // FIXED: Add <Program>
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Step1_Login_And_GetToken()
    {
        // Arrange
        var loginRequest = new
        {
            email = "admin@test.com",  // Note: your test uses admin@test.com, not admin@insurex.com
            password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(result?.token);
        
        _token = result.token;
        Console.WriteLine($"✅ Login successful. Token: {_token.Substring(0, Math.Min(20, _token.Length))}...");
    }

    [Fact]
    public async Task Step2_Try_Different_Asset_Endpoints()
    {
        await Step1_Login_And_GetToken();
        
        // Set the authorization header
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

        // Try different possible asset endpoints - include versioned ones
        var endpointsToTry = new[]
        {
            "/api/v1/assets",      // Most likely (from your controller)
            "/api/v1/asset",
            "/api/assets",
            "/api/asset",
            "/api/v2/assets"
        };

        foreach (var endpoint in endpointsToTry)
        {
            try
            {
                Console.WriteLine($"\n🔍 Trying: {endpoint}");
                
                // Try a simple GET first
                var getResponse = await _client.GetAsync(endpoint);
                Console.WriteLine($"   GET {endpoint}: {(int)getResponse.StatusCode} {getResponse.StatusCode}");
                
                if (getResponse.IsSuccessStatusCode)
                {
                    var content = await getResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ✅ Found working endpoint! Response length: {content.Length} chars");
                }

                // Try POST with minimal valid data
                if (getResponse.StatusCode == HttpStatusCode.OK || 
                    getResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine($"   Testing POST to {endpoint}...");
                    
                    var testAsset = new
                    {
                        assetType = "Vehicle",
                        description = "Test Asset",
                        policyId = 1,  // Assuming policy ID 1 exists
                        financeValue = 10000M,
                        insuredValue = 12000M,
                        details = new Dictionary<string, object>
                        {
                            ["make"] = "Test",
                            ["model"] = "Test"
                        }
                    };
                    
                    var postResponse = await _client.PostAsJsonAsync(endpoint, testAsset);
                    Console.WriteLine($"   POST {endpoint}: {(int)postResponse.StatusCode} {postResponse.StatusCode}");
                    
                    if (postResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"   ✅ POST works at {endpoint}!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
            }
        }
    }
}

public class LoginResponse
{
    public string token { get; set; } = "";
    public string email { get; set; } = "";
    public string role { get; set; } = "";
}