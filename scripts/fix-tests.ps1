# fix-tests.ps1
Write-Host "🔧 Fixing Integration Tests..." -ForegroundColor Cyan

# Stop any running processes
Write-Host "Step 1: Stopping processes..." -ForegroundColor Yellow
Get-Process | Where-Object { $_.ProcessName -like "*dotnet*" } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Navigate to Integration Tests folder
cd C:\Users\cluiz\code-base-Insurex\InsureX.IntegrationTests

# Step 2: Backup current tests
Write-Host "Step 2: Backing up test files..." -ForegroundColor Yellow
$backupDir = "C:\Users\cluiz\code-base-Insurex\TestBackup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
Copy-Item "*.cs" -Destination $backupDir
Write-Host "  ✅ Backed up to: $backupDir"

# Step 3: Remove duplicate test files
Write-Host "Step 3: Removing duplicate test files..." -ForegroundColor Yellow
Remove-Item -Path "FindAssetEndpoint.cs" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "FindAssetEndpointTests.cs" -Force -ErrorAction SilentlyContinue
Write-Host "  ✅ Removed conflicting files"

# Step 4: Create a clean, consolidated test file
Write-Host "Step 4: Creating clean test file..." -ForegroundColor Yellow

@"
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class AssetEndpointTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;

    public AssetEndpointTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
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
        Assert.True(response.IsSuccessStatusCode, $"Login failed: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        
        // Store token for subsequent tests
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Token);
    }

    [Fact]
    public async Task Step2_Get_All_Assets()
    {
        // First login to get token
        await Step1_Login_And_GetToken();
        
        // Act - Get all assets
        var response = await _client.GetAsync("/api/assets");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Get assets failed: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(content));
    }

    [Fact]
    public async Task Step3_Create_New_Asset()
    {
        // First login to get token
        await Step1_Login_And_GetToken();
        
        // Arrange - Create a vehicle asset
        var newAsset = new
        {
            policyId = 1,
            assetType = "Vehicle",
            assetData = new
            {
                Make = "Tesla",
                Model = "Model 3",
                Year = 2023,
                VIN = "5YJ3E1EAXPF123456",
                Registration = "ABC-123",
                FinanceValue = 45000.00m,
                InsuredValue = 50000.00m
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/assets", newAsset);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Create asset failed: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(content));
    }
}
"@ | Out-File -FilePath "AssetEndpointTests.cs" -Encoding UTF8

# Step 5: Check if TestWebApplicationFactory needs fixing
Write-Host "Step 5: Checking TestWebApplicationFactory..." -ForegroundColor Yellow
$factoryFile = "TestWebApplicationFactory.cs"
if (Test-Path $factoryFile) {
    $content = Get-Content $factoryFile -Raw
    if ($content -notmatch "class TestWebApplicationFactory<TProgram>") {
        # Create proper factory
        @"
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace InsureX.IntegrationTests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Add any test-specific service configuration here
        });
        
        return base.CreateHost(builder);
    }
}
"@ | Out-File -FilePath $factoryFile -Encoding UTF8
        Write-Host "  ✅ Fixed TestWebApplicationFactory"
    } else {
        Write-Host "  ✅ TestWebApplicationFactory looks good"
    }
}

# Step 6: Clean and rebuild
Write-Host "Step 6: Cleaning and rebuilding..." -ForegroundColor Yellow
Remove-Item -Path "bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "obj" -Recurse -Force -ErrorAction SilentlyContinue

cd C:\Users\cluiz\code-base-Insurex\InsureX.ModernAPI
Remove-Item -Path "bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "obj" -Recurse -Force -ErrorAction SilentlyContinue
dotnet build

cd C:\Users\cluiz\code-base-Insurex\InsureX.IntegrationTests
dotnet build

# Step 7: Run the tests
Write-Host "Step 7: Running tests..." -ForegroundColor Yellow
dotnet test

Write-Host "`n✅ Fix process complete!" -ForegroundColor Green
Write-Host "Your test files are backed up at: $backupDir" -ForegroundColor Yellow
