# check-status.ps1
Write-Host "=== INSUREX PROJECT STATUS ===" -ForegroundColor Magenta
Write-Host "Last checked: 03/01/2026 11:49:20
"

# Check Modern API
if (Test-Path "InsureX.ModernAPI") {
    Write-Host "✅ Modern API: Present" -ForegroundColor Green
    Set-Location InsureX.ModernAPI
    dotnet build --no-restore --nologo -v q > $null 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   - Build: ✅ Passing" -ForegroundColor Green
    } else {
        Write-Host "   - Build: ❌ Failing" -ForegroundColor Red
    }
    Set-Location ..
} else {
    Write-Host "❌ Modern API: Missing" -ForegroundColor Red
}

# Check React app
if (Test-Path "insurex-react-app") {
    Write-Host "
✅ React App: Present" -ForegroundColor Green
    if (Test-Path "insurex-react-app/node_modules") {
        Write-Host "   - Dependencies: ✅ Installed" -ForegroundColor Green
    } else {
        Write-Host "   - Dependencies: ❌ Missing (run npm install)" -ForegroundColor Yellow
    }
} else {
    Write-Host "
❌ React App: Missing" -ForegroundColor Red
}

# Check database
$tables = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM sys.tables" -h -1 2>$null
if ($tables -and $tables -gt 0) {
    Write-Host "
✅ Database: Present ($tables tables)" -ForegroundColor Green
    $policies = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM Policies" -h -1 2>$null
    Write-Host "   - Policies: $policies" -ForegroundColor Cyan
    $users = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM Users" -h -1 2>$null
    Write-Host "   - Users: $users" -ForegroundColor Cyan
} else {
    Write-Host "
❌ Database: Not found" -ForegroundColor Red
}

Write-Host "
=== NEXT ACTIONS ===" -ForegroundColor Cyan
Write-Host "1. Run integration tests (cd InsureX.IntegrationTests; dotnet test)"
Write-Host "2. Start React app (cd insurex-react-app; npm start)"
Write-Host "3. Test Docker (docker-compose up --build)"
Write-Host "4. Complete Claims module"
Write-Host "
Run: dotnet run --project InsureX.ModernAPI to start API"
Read-Host "
Press Enter to exit"
