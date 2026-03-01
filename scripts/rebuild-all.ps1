Write-Host "=== STOPPING ALL RUNNING PROCESSES ===" -ForegroundColor Magenta

# Stop all dotnet processes
Write-Host "`n1. Stopping dotnet processes..." -ForegroundColor Cyan
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "   Stopping dotnet process ID: $($_.Id)" -ForegroundColor Yellow
    Stop-Process -Id $_.Id -Force
}

# Stop any InsureX processes
Get-Process | Where-Object { $_.ProcessName -like "*InsureX*" } -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "   Stopping $($_.ProcessName) process ID: $($_.Id)" -ForegroundColor Yellow
    Stop-Process -Id $_.Id -Force
}

# Wait a moment for processes to fully stop
Start-Sleep -Seconds 2

# Clean the solution
Write-Host "`n2. Cleaning solution..." -ForegroundColor Cyan
dotnet clean

# Build the solution
Write-Host "`n3. Building solution..." -ForegroundColor Cyan
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
        Write-Host "   Check the test output above for details" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ Build failed. Please check the error messages above." -ForegroundColor Red
}

Write-Host "`n=== PROCESS COMPLETE ===" -ForegroundColor Magenta
Write-Host ""
Write-Host "To start the API again:"
Write-Host "  cd InsureX.ModernAPI"
Write-Host "  dotnet run"
Write-Host ""
Write-Host "To start React app:"
Write-Host "  cd insurex-react-app"
Write-Host "  npm run dev"
