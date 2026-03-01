# test-api.ps1
try {
    Write-Host "Attempting to login..." -ForegroundColor Yellow
     = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" 
        -Method Post 
        -ContentType "application/json" 
        -Body '{"email":"admin@insurex.com","password":"password"}'
    
     = .token
    Write-Host "✓ Login successful!" -ForegroundColor Green
    Write-Host "Token: " -ForegroundColor Cyan
    
    Write-Host "
Fetching policies..." -ForegroundColor Yellow
     = Invoke-RestMethod -Uri "http://localhost:5000/api/policies" 
        -Headers @{Authorization = "Bearer "}
    
    Write-Host "✓ Policies retrieved successfully!" -ForegroundColor Green
    Write-Host "Policies: "
}
catch {
    Write-Host "✗ Error: " -ForegroundColor Red
    if (.Exception.Response) {
         = .Exception.Response.GetResponseStream()
         = New-Object System.IO.StreamReader()
         = .ReadToEnd()
        Write-Host "Response Body: " -ForegroundColor Red
    }
}
Read-Host "
Press Enter to exit"
