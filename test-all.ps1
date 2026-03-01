
Write-Host "=== TESTING ALL ENDPOINTS ===" -ForegroundColor Magenta

$baseUrl = "http://localhost:5012"

# 1. Login to get token
Write-Host "`n1. Logging in..." -ForegroundColor Yellow
$loginBody = @{
    email = "admin@insurex.com"
    password = "password"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
        -Method Post `
        -ContentType "application/json" `
        -Body $loginBody
    
    $token = $loginResponse.token
    Write-Host "   ✅ Login successful!" -ForegroundColor Green
    Write-Host "   Token: $($token.Substring(0, 20))..." -ForegroundColor Gray
    
    # Headers for authenticated requests
    $headers = @{ Authorization = "Bearer $token" }
    
    # 2. Get all policies (should be empty initially)
    Write-Host "`n2. Getting policies..." -ForegroundColor Yellow
    $policies = Invoke-RestMethod -Uri "$baseUrl/api/policies" -Headers $headers
    Write-Host "   ✅ Policies retrieved: $($policies.Count)" -ForegroundColor Green
    
    # 3. Create a test policy
    Write-Host "`n3. Creating test policy..." -ForegroundColor Yellow
    $newPolicy = @{
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
    
    $createdPolicy = Invoke-RestMethod -Uri "$baseUrl/api/policies" `
        -Method Post `
        -Headers $headers `
        -ContentType "application/json" `
        -Body $newPolicy
    
    Write-Host "   ✅ Policy created: $($createdPolicy.policyNumber)" -ForegroundColor Green
    
    # 4. Get the created policy by ID
    Write-Host "`n4. Getting policy by ID..." -ForegroundColor Yellow
    $policy = Invoke-RestMethod -Uri "$baseUrl/api/policies/$($createdPolicy.id)" -Headers $headers
    Write-Host "   ✅ Policy retrieved: $($policy.policyHolder)" -ForegroundColor Green
    
    # 5. Get policy assets (should be empty)
    Write-Host "`n5. Getting policy assets..." -ForegroundColor Yellow
    $assets = Invoke-RestMethod -Uri "$baseUrl/api/policies/$($createdPolicy.id)/assets" -Headers $headers
    Write-Host "   ✅ Assets retrieved: $($assets.Count)" -ForegroundColor Green
    
    # 6. Get policy claims (should be empty)
    Write-Host "`n6. Getting policy claims..." -ForegroundColor Yellow
    $claims = Invoke-RestMethod -Uri "$baseUrl/api/policies/$($createdPolicy.id)/claims" -Headers $headers
    Write-Host "   ✅ Claims retrieved: $($claims.Count)" -ForegroundColor Green
    
    # 7. Get policy transactions (should be empty)
    Write-Host "`n7. Getting policy transactions..." -ForegroundColor Yellow
    $transactions = Invoke-RestMethod -Uri "$baseUrl/api/policies/$($createdPolicy.id)/transactions" -Headers $headers
    Write-Host "   ✅ Transactions retrieved: $($transactions.Count)" -ForegroundColor Green
    
    Write-Host "`n=== ALL TESTS PASSED! ===" -ForegroundColor Green
}
catch {
    Write-Host "`n❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}
