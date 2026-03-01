using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace InsureX.IntegrationTests;

public class FindAssetEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private string _token;

    public FindAssetEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(result?.token);
        
        _token = result.token;
        Console.WriteLine($"✅ Login successful. Token: {_token.Substring(0, 20)}...");
    }

    [Fact]
    public async Task Step2_Try_Different_Asset_Endpoints()
    {
        await Step1_Login_And_GetToken();
        
        // Set the authorization header
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

        // Try different possible asset endpoints
        var endpointsToTry = new[]
        {
            "/api/assets",
            "/api/Asset",
            "/api/asset",
            "/api/Assets",
            "/assets",
            "/api/v1/assets",
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
                    Console.WriteLine($"   ✅ Found working endpoint! Response: {content.Substring(0, Math.Min(100, content.Length))}...");
                }

                // If GET works, try to understand the expected format
                if (getResponse.StatusCode == HttpStatusCode.OK || 
                    getResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine($"   ⚠️ This endpoint responds to GET. Check controller attributes.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
            }
        }
    }

    [Fact]
    public async Task Step3_Check_Controller_Attributes_In_Code()
    {
        // This test will just remind you to check the actual controller code
        Console.WriteLine("\n🔍 Manual Step: Check your AssetController.cs for the route attribute:");
        Console.WriteLine("   Look for lines like:");
        Console.WriteLine("   [Route(\"api/[controller]\")]");
        Console.WriteLine("   [Route(\"api/assets\")]");
        Console.WriteLine("   [ApiController]");
        Console.WriteLine("   public class AssetController : ControllerBase");
        
        Assert.True(true); // Always pass
    }
}

// Add this class at the bottom of the file
public class LoginResponse
{
    public string token { get; set; }
    public string email { get; set; }
    public string role { get; set; }
}