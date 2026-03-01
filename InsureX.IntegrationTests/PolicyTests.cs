using System.Net;
using System.Net.Http.Json;
using InsureX.ModernAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace InsureX.IntegrationTests
{
    public class PolicyTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PolicyTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
            // Get auth token first
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetAuthToken());
        }

        private string GetAuthToken()
        {
            // Login and get token
            var loginResponse = _client.PostAsJsonAsync("/api/auth/login", new 
            { 
                email = "admin@insurex.com", 
                password = "password" 
            }).Result;
            
            var token = loginResponse.Content.ReadFromJsonAsync<dynamic>().Result.token;
            return token;
        }

        [Fact]
        public async Task GetAllPolicies_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/api/policies");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var policies = await response.Content.ReadFromJsonAsync<List<Policy>>();
            Assert.NotNull(policies);
            Assert.True(policies.Count >= 10); // We added 10+ policies
        }

        [Fact]
        public async Task GetPolicyById_WithValidId_ReturnsPolicy()
        {
            // Arrange
            var policyId = 1; // First policy
            
            // Act
            var response = await _client.GetAsync($"/api/policies/{policyId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var policy = await response.Content.ReadFromJsonAsync<Policy>();
            Assert.NotNull(policy);
            Assert.Equal(policyId, policy.Id);
        }

        [Fact]
        public async Task CreatePolicy_WithValidData_ReturnsCreated()
        {
            // Arrange
            var newPolicy = new
            {
                PolicyNumber = "POL-TEST-001",
                CustomerName = "Test Customer",
                CustomerEmail = "test@example.com",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1),
                Status = "Pending",
                TotalInsuredValue = 1000000.00M
            };
            
            // Act
            var response = await _client.PostAsJsonAsync("/api/policies", newPolicy);
            
            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<Policy>();
            Assert.NotNull(created);
            Assert.Equal(newPolicy.PolicyNumber, created.PolicyNumber);
        }
    }
}