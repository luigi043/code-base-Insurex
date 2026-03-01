# Save this as check-endpoints.ps1
Write-Host "🔍 Checking API Endpoints..." -ForegroundColor Cyan

# Start the API in background
cd C:\Users\cluiz\code-base-Insurex\InsureX.ModernAPI
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run" -PassThru -NoNewWindow
Start-Sleep -Seconds 5

# Test endpoints
Write-Host "`n📡 Testing endpoints:" -ForegroundColor Yellow

# Test 1: Login endpoint
Write-Host "`n1. Testing Login endpoint..." -ForegroundColor Yellow
try {
    $loginBody = @{
        email = "admin@insurex.com"
        password = "password"
    } | ConvertTo-Json
    
    $response = Invoke-RestMethod -Uri "http://localhost:5012/api/auth/login" `
        -Method Post `
        -ContentType "application/json" `
        -Body $loginBody `
        -ErrorVariable apiError
    
    Write-Host "   ✅ Login successful! Token received" -ForegroundColor Green
    $token = $response.token
} catch {
    Write-Host "   ❌ Login failed: $($_.Exception.Message)" -ForegroundColor Red
    
    # Try alternative login endpoints
    Write-Host "   Trying alternative endpoints..." -ForegroundColor Yellow
    
    $altEndpoints = @(
        "http://localhost:5012/api/auth/login",
        "http://localhost:5012/auth/login",
        "http://localhost:5012/api/User/login",
        "http://localhost:5012/api/Account/login"
    )
    
    foreach ($endpoint in $altEndpoints) {
        try {
            Write-Host "   Testing: $endpoint" -NoNewline
            $response = Invoke-RestMethod -Uri $endpoint -Method Post -ContentType "application/json" -Body $loginBody -ErrorAction Stop
            Write-Host " ✅" -ForegroundColor Green
            $token = $response.token
            Write-Host "   ✅ Found working login endpoint: $endpoint" -ForegroundColor Green
            break
        } catch {
            Write-Host " ❌" -ForegroundColor Red
        }
    }
}

# Test 2: Get all endpoints via Swagger
Write-Host "`n2. Checking Swagger UI..." -ForegroundColor Yellow
try {
    $swagger = Invoke-WebRequest -Uri "http://localhost:5012/swagger/v1/swagger.json" -ErrorAction Stop
    $swaggerJson = $swagger.Content | ConvertFrom-Json
    Write-Host "   ✅ Swagger available!" -ForegroundColor Green
    
    # Extract all asset endpoints
    Write-Host "`n   Available Asset Endpoints:" -ForegroundColor Cyan
    $swaggerJson.paths.PSObject.Properties | Where-Object { $_.Name -like "*asset*" } | ForEach-Object {
        Write-Host "   $($_.Name)" -ForegroundColor White
    }
} catch {
    Write-Host "   ❌ Swagger not available: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Get policies (to find a valid policy ID)
if ($token) {
    Write-Host "`n3. Testing Policies endpoint..." -ForegroundColor Yellow
    try {
        $policies = Invoke-RestMethod -Uri "http://localhost:5012/api/policies" `
            -Headers @{Authorization = "Bearer $token"} `
            -ErrorAction Stop
        Write-Host "   ✅ Got policies! Found $($policies.Count) policies" -ForegroundColor Green
        
        if ($policies.Count -gt 0) {
            $firstPolicy = $policies[0]
            Write-Host "   First policy ID: $($firstPolicy.id)" -ForegroundColor Cyan
        }
    } catch {
        Write-Host "   ❌ Failed to get policies: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Cleanup
Stop-Process -Id $apiProcess.Id -Force -ErrorAction SilentlyContinue

Write-Host "`n✅ Endpoint check complete!" -ForegroundColor Green