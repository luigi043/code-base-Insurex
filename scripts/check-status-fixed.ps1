Write-Host "=== INSUREX PROJECT STATUS ===" -ForegroundColor Magenta
Write-Host "Last checked: $(Get-Date -Format 'MM/dd/yyyy HH:mm:ss')"
Write-Host ""

# Check Modern API
if (Test-Path "InsureX.ModernAPI") {
    Write-Host "✅ Modern API: Present" -ForegroundColor Green
    Push-Location InsureX.ModernAPI
    $buildResult = dotnet build --nologo -v q 2>&1
    Pop-Location
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   - Build: ✅ Passing" -ForegroundColor Green
    } else {
        Write-Host "   - Build: ❌ Failing" -ForegroundColor Red
    }
} else {
    Write-Host "❌ Modern API: Missing" -ForegroundColor Red
}

# Check React app
if (Test-Path "insurex-react-app") {
    Write-Host "`n✅ React App: Present" -ForegroundColor Green
    if (Test-Path "insurex-react-app/node_modules") {
        Write-Host "   - Dependencies: ✅ Installed" -ForegroundColor Green
    } else {
        Write-Host "   - Dependencies: ⚠️ Missing (run npm install)" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n❌ React App: Missing" -ForegroundColor Red
}

# Check database
try {
    $tables = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM sys.tables" -h -1 2>$null
    if ($tables -and $tables -gt 0) {
        Write-Host "`n✅ Database: Present ($tables tables)" -ForegroundColor Green
        $policies = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM Policies" -h -1 2>$null
        Write-Host "   - Policies: $policies" -ForegroundColor Cyan
        $assets = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM Assets" -h -1 2>$null
        Write-Host "   - Assets: $assets" -ForegroundColor Cyan
        $users = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM Users" -h -1 2>$null
        Write-Host "   - Users: $users" -ForegroundColor Cyan
    } else {
        Write-Host "`n❌ Database: Not found" -ForegroundColor Red
    }
} catch {
    Write-Host "`n❌ Database: Cannot connect - SQL Server may not be running" -ForegroundColor Red
}

# Check integration tests
if (Test-Path "InsureX.IntegrationTests") {
    Write-Host "`n✅ Integration Tests: Present" -ForegroundColor Green
    Push-Location InsureX.IntegrationTests
    $testCount = (dotnet test --nologo -v q | Select-String "Total tests:").ToString()
    Pop-Location
    if ($testCount) {
        Write-Host "   - Tests: $testCount" -ForegroundColor Cyan
    }
}

Write-Host "`n=== NEXT ACTIONS ===" -ForegroundColor Cyan
Write-Host "1. Complete React frontend (cd insurex-react-app; npm start)"
Write-Host "2. Add more integration tests (cd InsureX.IntegrationTests)"
Write-Host "3. Complete Claims module UI"
Write-Host "4. Complete Billing module"
Write-Host "`nRun: dotnet run --project InsureX.ModernAPI to start API"
Read-Host "`nPress Enter to exit"
