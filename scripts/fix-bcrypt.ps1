Write-Host "=== FIXING PACKAGE VERSION CONFLICT ===" -ForegroundColor Magenta

cd C:\Users\cluiz\code-base-Insurex\InsureX.IntegrationTests

# Remove the wrong version
Write-Host "`n1. Removing wrong package version..." -ForegroundColor Cyan
dotnet remove package BCrypt.Net-Next

# Add the correct version (4.1.0)
Write-Host "`n2. Adding correct package version (4.1.0)..." -ForegroundColor Cyan
dotnet add package BCrypt.Net-Next --version 4.1.0

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
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ All tests passed!" -ForegroundColor Green
    } else {
        Write-Host "❌ Some tests failed" -ForegroundColor Red
    }
} else {
    Write-Host "❌ Build failed. Please check the error messages above." -ForegroundColor Red
}

Write-Host "`n=== FIXES COMPLETE ===" -ForegroundColor Magenta
