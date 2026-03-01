# Save as check-seed-data.ps1
Write-Host "🔍 Checking Seed Data..." -ForegroundColor Cyan

# Look through your seed data scripts
$seedFiles = Get-ChildItem -Path "C:\Users\cluiz\code-base-Insurex\Database\Scripts" -Filter "*.sql"

foreach ($file in $seedFiles) {
    Write-Host "`nChecking: $($file.Name)" -ForegroundColor Yellow
    $content = Get-Content $file.FullName
    
    # Look for user inserts
    $userLines = $content | Select-String -Pattern "INSERT INTO.*Users" -Context 0,10
    if ($userLines) {
        Write-Host "Found Users table insertion:" -ForegroundColor Cyan
        $userLines.Line
        $userLines.Context.PostContext
    }
    
    # Look for policy inserts
    $policyLines = $content | Select-String -Pattern "INSERT INTO.*Policies" -Context 0,5
    if ($policyLines) {
        Write-Host "`nFound Policies table insertion:" -ForegroundColor Cyan
        $policyLines.Line
        $policyLines.Context.PostContext
    }
}