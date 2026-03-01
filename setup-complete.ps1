#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Complete setup script for InsureX project
.DESCRIPTION
    Sets up database, runs migrations, seeds data, and verifies installation
#>

param(
    [switch]$Clean,
    [switch]$SkipTests
)

Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force
$ErrorActionPreference = "Stop"
$rootDir = Get-Location

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   INSUREX - COMPLETE SETUP SCRIPT    " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Check prerequisites
Write-Host "🔍 Checking prerequisites..." -ForegroundColor Yellow

$hasDotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $hasDotnet) {
    Write-Host "❌ .NET SDK not found. Please install .NET 8 SDK" -ForegroundColor Red
    exit 1
}

$hasNode = Get-Command node -ErrorAction SilentlyContinue
if (-not $hasNode) {
    Write-Host "⚠️ Node.js not found. React frontend may not work" -ForegroundColor Yellow
}

Write-Host "✅ Prerequisites checked" -ForegroundColor Green
Write-Host ""

# 2. Clean if requested
if ($Clean) {
    Write-Host "🧹 Cleaning solution..." -ForegroundColor Yellow
    dotnet clean
    if (Test-Path "InsureX.ModernAPI/Migrations") {
        Remove-Item -Path "InsureX.ModernAPI/Migrations/*" -Recurse -Force -ErrorAction SilentlyContinue
    }
    Write-Host "✅ Clean completed" -ForegroundColor Green
    Write-Host ""
}

# 3. Restore and build
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet restore
dotnet build --configuration Release --no-restore
Write-Host "✅ Build completed" -ForegroundColor Green
Write-Host ""

# 4. Database setup
Write-Host "🗄️ Setting up database..." -ForegroundColor Yellow
cd InsureX.ModernAPI

# Apply migrations
Write-Host "  Applying EF Core migrations..." -ForegroundColor Gray
dotnet ef database drop --force -v --quiet 2>$null
dotnet ef database update -v --quiet
Write-Host "  ✅ Migrations applied" -ForegroundColor Green

# Run seed script
Write-Host "  Loading seed data..." -ForegroundColor Gray
$connectionString = "Server=(localdb)\mssqllocaldb;Database=InsureX;Trusted_Connection=True;TrustServerCertificate=true"
$sqlFile = Join-Path $rootDir "Database/Scripts/03 - Seed Data.sql"
if (Test-Path $sqlFile) {
    sqlcmd -S "(localdb)\mssqllocaldb" -d "InsureX" -i "$sqlFile" -b -C
    Write-Host "  ✅ Seed data loaded" -ForegroundColor Green
} else {
    Write-Host "  ⚠️ Seed data file not found" -ForegroundColor Yellow
}

cd $rootDir
Write-Host "✅ Database setup completed" -ForegroundColor Green
Write-Host ""

# 5. Run tests (unless skipped)
if (-not $SkipTests) {
    Write-Host "🧪 Running tests..." -ForegroundColor Yellow
    dotnet test InsureX.IntegrationTests/InsureX.IntegrationTests.csproj --no-build --verbosity quiet
    Write-Host "✅ Tests completed" -ForegroundColor Green
    Write-Host ""
}

# 6. React setup
if ($hasNode) {
    Write-Host "⚛️ Setting up React frontend..." -ForegroundColor Yellow
    if (Test-Path "insurex-react-app") {
        cd insurex-react-app
        npm install --silent
        Write-Host "✅ React dependencies installed" -ForegroundColor Green
        cd $rootDir
    }
} else {
    Write-Host "⚠️ Skipping React setup (Node.js not found)" -ForegroundColor Yellow
}

# 7. Final status
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   ✅ SETUP COMPLETE!                   " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📝 Next steps:" -ForegroundColor Cyan
Write-Host "  1. Start the API:         cd InsureX.ModernAPI; dotnet run" -ForegroundColor White
Write-Host "  2. API will be at:        https://localhost:5012" -ForegroundColor White
Write-Host "  3. View Swagger docs:     https://localhost:5012/swagger" -ForegroundColor White
Write-Host "  4. Start React app:       cd insurex-react-app; npm start" -ForegroundColor White
Write-Host "  5. React app at:          http://localhost:3000" -ForegroundColor White
Write-Host ""
Write-Host "📊 Test credentials:" -ForegroundColor Cyan
Write-Host "  Admin:   admin@insurex.com / password" -ForegroundColor White
Write-Host "  Financer: john@fnb.co.za / password" -ForegroundColor White
Write-Host "  Insurer:  sarah@standardinsure.com / password" -ForegroundColor White