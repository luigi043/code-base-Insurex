# fix-everything.ps1
Write-Host "=== FIXING INSUREX MODERN API ===" -ForegroundColor Magenta

# Step 1: Navigate to ModernAPI
Set-Location -Path "InsureX.ModernAPI"

# Step 2: Create/Update models
Write-Host "
[1/7] Creating/Updating models..." -ForegroundColor Yellow
# (Include all the model creation commands from above)

# Step 3: Update DbContext
Write-Host "
[2/7] Updating DbContext..." -ForegroundColor Yellow
# (Include DbContext creation command)

# Step 4: Build
Write-Host "
[3/7] Building project..." -ForegroundColor Yellow
dotnet build

if ( -eq 0) {
    # Step 5: Drop database
    Write-Host "
[4/7] Dropping existing database..." -ForegroundColor Yellow
    dotnet ef database drop --force
    
    # Step 6: Remove migrations
    Write-Host "
[5/7] Removing old migrations..." -ForegroundColor Yellow
    dotnet ef migrations remove -f
    
    # Step 7: Create new migration
    Write-Host "
[6/7] Creating new migration..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate_Final
    
    # Step 8: Update database
    Write-Host "
[7/7] Applying migration..." -ForegroundColor Yellow
    dotnet ef database update
    
    Write-Host "
✓ ALL DONE! Run 'dotnet run' to start the API" -ForegroundColor Green
} else {
    Write-Host "
✗ Build failed. Please check errors above." -ForegroundColor Red
}
