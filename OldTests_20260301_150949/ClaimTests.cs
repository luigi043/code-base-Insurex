using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class ClaimTests : BaseTest
{
    public ClaimTests(TestWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateClaim_ReturnsSuccess()
    {
        // Arrange
        var newClaim = new
        {
            policyId = _testPolicyId,
            claimDate = DateTime.UtcNow,
            description = "Test claim from integration test",
            claimAmount = 5000.00M,
            notes = "Test notes"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/claims", newClaim);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var claim = await response.Content.ReadFromJsonAsync<ClaimDto>();
        Assert.NotNull(claim);
        Assert.Equal(_testPolicyId, claim.PolicyId);
        Assert.Equal("Submitted", claim.Status);
    }

    [Fact]
    public async Task GetClaimsByPolicy_ReturnsClaims()
    {
        // First create a claim
        await _client.PostAsJsonAsync("/api/claims", new
        {
            policyId = _testPolicyId,
            claimDate = DateTime.UtcNow,
            description = "Test claim for policy",
            claimAmount = 3000.00M
        });

        // Act
        var response = await _client.GetAsync($"/api/claims/policy/{_testPolicyId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var claims = await response.Content.ReadFromJsonAsync<List<ClaimDto>>();
        Assert.NotNull(claims);
        Assert.NotEmpty(claims);
    }

    private class ClaimDto
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = "";
        public int PolicyId { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Description { get; set; } = "";
        public decimal ClaimAmount { get; set; }
        public string Status { get; set; } = "";
    }
}
