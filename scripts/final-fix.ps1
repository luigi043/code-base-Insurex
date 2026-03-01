Write-Host "=== FINAL FIXES FOR TESTS ===" -ForegroundColor Magenta

cd C:\Users\cluiz\code-base-Insurex\InsureX.IntegrationTests

# Add BCrypt package
Write-Host "`n1. Adding BCrypt package..." -ForegroundColor Cyan
dotnet add package BCrypt.Net-Next --version 4.0.3

# Restore packages
Write-Host "`n2. Restoring packages..." -ForegroundColor Cyan
dotnet restore

# Build
Write-Host "`n3. Building..." -ForegroundColor Cyan
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
    
    # Run tests
    Write-Host "`n4. Running tests..." -ForegroundColor Cyan
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
