Write-Host "=== COMPLETE RESET WITH CORRECT VERSIONS ===" -ForegroundColor Magenta

cd C:\Users\cluiz\code-base-Insurex\InsureX.IntegrationTests

# Remove all existing files
Write-Host "`n1. Cleaning up..." -ForegroundColor Cyan
Remove-Item -Path "obj" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "*.csproj" -Force -ErrorAction SilentlyContinue

# Create new project file
Write-Host "`n2. Creating fresh project file..." -ForegroundColor Cyan
@'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.4">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="BCrypt.Net-Next" Version="4.1.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\InsureX.ModernAPI\InsureX.ModernAPI.csproj" />
  </ItemGroup>

</Project>
'@ | Out-File -FilePath "InsureX.IntegrationTests.csproj" -Encoding utf8

# Restore packages
Write-Host "`n3. Restoring packages..." -ForegroundColor Cyan
dotnet restore

# Build
Write-Host "`n4. Building..." -ForegroundColor Cyan
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
    
    # Run tests
    Write-Host "`n5. Running tests..." -ForegroundColor Cyan
    dotnet test
} else {
    Write-Host "❌ Build failed. Please check the error messages above." -ForegroundColor Red
}

Write-Host "`n=== RESET COMPLETE ===" -ForegroundColor Magenta