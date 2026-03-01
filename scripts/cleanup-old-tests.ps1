# Save as cleanup-old-tests.ps1
Write-Host "🧹 Cleaning up old test files..." -ForegroundColor Cyan

cd C:\Users\cluiz\code-base-Insurex\InsureX.IntegrationTests

# List of files to keep
$keepFiles = @(
    "CorrectAssetTests.cs",
    "TestWebApplicationFactory.cs",
    "Usings.cs",
    "InsureX.IntegrationTests.csproj"
)

# Move all other .cs files to backup
$backupDir = "C:\Users\cluiz\code-base-Insurex\OldTests_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null

Get-ChildItem -Path "*.cs" | Where-Object { $_.Name -notin $keepFiles } | ForEach-Object {
    Write-Host "  Moving: $($_.Name)" -ForegroundColor Yellow
    Move-Item -Path $_.FullName -Destination "$backupDir\$($_.Name)" -Force
}

Write-Host "`n✅ Old test files moved to: $backupDir" -ForegroundColor Green