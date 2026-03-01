# test-all.ps1
Write-Host "=== TESTING ALL ENDPOINTS ===" -ForegroundColor Magenta

 = "http://localhost:5012"

# 1. Login to get token
Write-Host "
1. Logging in..." -ForegroundColor Yellow
 = @{
    email = "admin@insurex.com"
    password = "password"
} | ConvertTo-Json

try {
     = Invoke-RestMethod -Uri "/api/auth/login" 
        -Method Post 
        -ContentType "application/json" 
        -Body 
    
     = .token
    Write-Host "   ✅ Login successful!" -ForegroundColor Green
    Write-Host "   Token: ..." -ForegroundColor Gray
    
    # Headers for authenticated requests
     = @{ Authorization = "Bearer " }
    
    # 2. Get all policies (should be empty initially)
    Write-Host "
2. Getting policies..." -ForegroundColor Yellow
     = Invoke-RestMethod -Uri "/api/policies" -Headers 
    Write-Host "   ✅ Policies retrieved: 0" -ForegroundColor Green
    
    # 3. Create a test policy
    Write-Host "
3. Creating test policy..." -ForegroundColor Yellow
     = @{
        policyNumber = "TEST-001"
        policyHolder = "Test Client"
        email = "test@example.com"
        phone = "123-456-7890"
        startDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
        endDate = (Get-Date).AddYears(1).ToString("yyyy-MM-ddTHH:mm:ssZ")
        status = "Active"
        premium = 1000.00
        policyType = "Business"
        notes = "Test policy"
    } | ConvertTo-Json
    
     = Invoke-RestMethod -Uri "/api/policies" 
        -Method Post 
        -Headers  
        -ContentType "application/json" 
        -Body 
    
    Write-Host "   ✅ Policy created: " -ForegroundColor Green
    
    # 4. Get the created policy by ID
    Write-Host "
4. Getting policy by ID..." -ForegroundColor Yellow
     = Invoke-RestMethod -Uri "/api/policies/" -Headers 
    Write-Host "   ✅ Policy retrieved: " -ForegroundColor Green
    
    # 5. Get policy assets (should be empty)
    Write-Host "
5. Getting policy assets..." -ForegroundColor Yellow
     = Invoke-RestMethod -Uri "/api/policies//assets" -Headers 
    Write-Host "   ✅ Assets retrieved: 0" -ForegroundColor Green
    
    # 6. Get policy claims (should be empty)
    Write-Host "
6. Getting policy claims..." -ForegroundColor Yellow
     = Invoke-RestMethod -Uri "/api/policies//claims" -Headers 
    Write-Host "   ✅ Claims retrieved: 0" -ForegroundColor Green
    
    # 7. Get policy transactions (should be empty)
    Write-Host "
7. Getting policy transactions..." -ForegroundColor Yellow
     = Invoke-RestMethod -Uri "/api/policies//transactions" -Headers 
    Write-Host "   ✅ Transactions retrieved: 0" -ForegroundColor Green
    
    Write-Host "
=== ALL TESTS PASSED! ===" -ForegroundColor Green
}
catch {
    Write-Host "
❌ Error: " -ForegroundColor Red
    if (.Exception.Response) {
         = .Exception.Response.GetResponseStream()
         = New-Object System.IO.StreamReader()
         = .ReadToEnd()
        Write-Host "Response Body: " -ForegroundColor Red
    }
}

Read-Host "
Press Enter to exit"
