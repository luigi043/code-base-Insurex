using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace InsureX.IntegrationTests
{
    public abstract class BaseTest : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
    {
        protected readonly HttpClient _client;
        protected readonly TestWebApplicationFactory<Program> _factory;
        protected string? _token;
        protected int _testPolicyId;
        protected bool _isInitialized = false;

        protected BaseTest(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                await LoginAsync();
                await EnsurePolicyExistsAsync();
                _isInitialized = true;
            }
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            return Task.CompletedTask;
        }

        protected async Task LoginAsync()
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
                new AuthenticationHeaderValue("Bearer", _token);
        }

        protected async Task EnsurePolicyExistsAsync()
        {
            // First try to get existing policies
            var getResponse = await _client.GetAsync("/api/policies");
            getResponse.EnsureSuccessStatusCode();
            
            var policies = await getResponse.Content.ReadFromJsonAsync<List<PolicyDto>>();
            
            if (policies != null && policies.Any())
            {
                _testPolicyId = policies.First().Id;
                return;
            }

            // Create new policy if none exist
            var newPolicy = new
            {
                policyNumber = $"POL-TEST-{DateTime.Now.Ticks}",
                policyHolder = "Integration Test Customer",
                email = "test@example.com",
                phone = "123-456-7890",
                startDate = DateTime.UtcNow,
                endDate = DateTime.UtcNow.AddYears(1),
                status = "Active",
                premium = 1000.00M,
                policyType = "Comprehensive"
            };

            var response = await _client.PostAsJsonAsync("/api/policies", newPolicy);
            response.EnsureSuccessStatusCode();
            
            var policy = await response.Content.ReadFromJsonAsync<PolicyDto>();
            _testPolicyId = policy?.Id ?? 0;
            
            Assert.True(_testPolicyId > 0, "Failed to create test policy");
        }

        protected class LoginResult
        {
            public string Token { get; set; } = "";
        }

        protected class PolicyDto
        {
            public int Id { get; set; }
            public string PolicyNumber { get; set; } = "";
            public string PolicyHolder { get; set; } = "";
        }
    }
}
