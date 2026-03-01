# test-login.ps1
try {
    Write-Host "Attempting to login..." -ForegroundColor Yellow
    
     = @{
        email = "admin@insurex.com"
        password = "password"
    } | ConvertTo-Json
    
    Write-Host "Sending request to: http://localhost:5012/api/auth/login" -ForegroundColor Cyan
    Write-Host "Request body: " -ForegroundColor Gray
    
     = Invoke-RestMethod -Uri "http://localhost:5012/api/auth/login" 
        -Method Post 
        -ContentType "application/json" 
        -Body 
    
    Write-Host "
✅ Login successful!" -ForegroundColor Green
    Write-Host "Token: " -ForegroundColor Cyan
    Write-Host "User: " -ForegroundColor Green
    
    # Test a protected endpoint
    Write-Host "
Testing protected endpoint..." -ForegroundColor Yellow
    
     = Invoke-RestMethod -Uri "http://localhost:5012/api/policies" 
        -Headers @{Authorization = "Bearer "}
    
    Write-Host "✅ Protected endpoint accessed successfully!" -ForegroundColor Green
    Write-Host "Policies count: 0" -ForegroundColor Green
}
catch {
    Write-Host "
❌ Error: " -ForegroundColor Red
    
    if (.Exception.Response) {
         = .Exception.Response.GetResponseStream()
         = New-Object System.IO.StreamReader()
         = .ReadToEnd()
        Write-Host "Response Body: " -ForegroundColor Red
        
        # Try to parse the error
        try {
             =  | ConvertFrom-Json
            Write-Host "Error details: " -ForegroundColor Red
        }
        catch {
            # If not JSON, show raw response
        }
    }
}

Read-Host "
Press Enter to exit"
