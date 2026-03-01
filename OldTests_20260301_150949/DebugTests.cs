using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace InsureX.IntegrationTests;

public class DebugTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DebugTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Debug_PolicyEndpoints()
    {
        // First login
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
        var getResponse = await _client.GetAsync("/api/policies");
        getResponse.EnsureSuccessStatusCode();
        var policies = await getResponse.Content.ReadFromJsonAsync<List<PolicyDto>>();
        
        Assert.NotNull(policies);
        
        // Try to create a policy with minimal required fields
        var minimalPolicy = new
        {
            policyNumber = $"POL-DEBUG-{DateTime.Now.Ticks}",
            policyHolder = "Debug Customer",
            email = "debug@test.com",
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddYears(1),
            status = "Active"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/policies", minimalPolicy);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        
        Assert.True(createResponse.IsSuccessStatusCode, 
            $"Create failed: {createResponse.StatusCode} - {createContent}");
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
}
