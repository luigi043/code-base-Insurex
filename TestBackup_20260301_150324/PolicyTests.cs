using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class PolicyTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private string? _token;

    public PolicyTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task LoginAsync()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@test.com",
            password = "password"
        });

        loginResponse.EnsureSuccessStatusCode();
        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
        _token = result?.Token ?? "";
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    [Fact]
    public async Task GetAllPolicies_ReturnsSuccess()
    {
        await LoginAsync();
        
        // Act
        var response = await _client.GetAsync("/api/policies");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var policies = await response.Content.ReadFromJsonAsync<List<PolicyListItemDto>>();
        Assert.NotNull(policies);
    }

    [Fact]
    public async Task CreatePolicy_ReturnsCreatedPolicy()
    {
        await LoginAsync();

        // Arrange
        var newPolicy = new
        {
            policyNumber = $"POL-NEW-{DateTime.Now.Ticks}",
            policyHolder = "New Test Customer",
            email = "new@example.com",
            phone = "987-654-3210",
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddYears(1),
            status = "Active",
            premium = 1500.00M,
            policyType = "Premium"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/policies", newPolicy);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Create failed with status {response.StatusCode}: {error}");
        }

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var policy = await response.Content.ReadFromJsonAsync<PolicyDetailDto>();
        Assert.NotNull(policy);
        Assert.False(string.IsNullOrEmpty(policy.PolicyNumber));
        Assert.Equal(newPolicy.policyHolder, policy.PolicyHolder);
        Assert.Equal(newPolicy.premium, policy.Premium);
    }

    [Fact]
    public async Task GetPolicyById_ReturnsPolicy()
    {
        await LoginAsync();
        
        // First create a policy to get an ID
        var createResponse = await _client.PostAsJsonAsync("/api/policies", new
        {
            policyNumber = $"POL-GET-{DateTime.Now.Ticks}",
            policyHolder = "Get Test Customer",
            email = "get@example.com",
            phone = "123-456-7890",
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddYears(1),
            status = "Active",
            premium = 1000.00M,
            policyType = "Standard"
        });
        
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<PolicyDetailDto>();
        
        // Act
        var response = await _client.GetAsync($"/api/policies/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var policy = await response.Content.ReadFromJsonAsync<PolicyDetailDto>();
        Assert.NotNull(policy);
        Assert.Equal(created.Id, policy.Id);
    }

    [Fact]
    public async Task GetPolicyById_WithInvalidId_ReturnsNotFound()
    {
        await LoginAsync();
        
        // Act
        var response = await _client.GetAsync("/api/policies/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Temporarily comment out the update test until we figure out the exact API contract
    // [Fact]
    // public async Task UpdatePolicy_ReturnsSuccess() { ... }

    private class LoginResult
    {
        public string Token { get; set; } = "";
    }

    private class PolicyListItemDto
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = "";
        public string PolicyHolder { get; set; } = "";
        public string Status { get; set; } = "";
    }

    private class PolicyDetailDto
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = "";
        public string PolicyHolder { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "";
        public decimal Premium { get; set; }
        public string PolicyType { get; set; } = "";
    }
}
