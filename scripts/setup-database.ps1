# setup-database.ps1
Write-Host "=== DATABASE SETUP ===" -ForegroundColor Magenta

# Wait for API to create database
Write-Host "
Waiting for database to be created..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Run indexes
Write-Host "
Creating indexes..." -ForegroundColor Yellow
sqlcmd -S localhost -d InsureX -i "Database\Scripts\06 - Indexes.sql" -b > $null 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Indexes created" -ForegroundColor Green
} else {
    Write-Host "⚠️ Indexes may already exist or database not ready" -ForegroundColor Yellow
}

# Run seed data
Write-Host "
Inserting seed data..." -ForegroundColor Yellow
sqlcmd -S localhost -d InsureX -i "Database\Scripts\03 - Seed Data.sql" -b > $null 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Seed data inserted" -ForegroundColor Green
} else {
    Write-Host "⚠️ Seed data may already exist" -ForegroundColor Yellow
}

# Verify setup
Write-Host "
Verifying database..." -ForegroundColor Yellow
$tables = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM sys.tables" -h -1
Write-Host "   Tables: $tables" -ForegroundColor Cyan
$policies = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM Policies" -h -1
Write-Host "   Policies: $policies" -ForegroundColor Cyan
$assets = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM Assets" -h -1
Write-Host "   Assets: $assets" -ForegroundColor Cyan
$users = sqlcmd -S localhost -d InsureX -Q "SELECT COUNT(*) FROM Users" -h -1
Write-Host "   Users: $users" -ForegroundColor Cyan

Write-Host "
✅ Database setup complete!" -ForegroundColor Green
Read-Host "
Press Enter to exit"
