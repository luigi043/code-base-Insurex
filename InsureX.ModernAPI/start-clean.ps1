Write-Host "=== STOPPING ALL RUNNING INSTANCES ===" -ForegroundColor Magenta

# Stop all dotnet processes
Write-Host "`nStopping dotnet processes..." -ForegroundColor Cyan
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "Stopping dotnet process ID: $($_.Id)" -ForegroundColor Yellow
    Stop-Process -Id $_.Id -Force
}

# Stop any InsureX processes
Get-Process | Where-Object { $_.ProcessName -like "*InsureX*" } | ForEach-Object {
    Write-Host "Stopping $($_.ProcessName) process ID: $($_.Id)" -ForegroundColor Yellow
    Stop-Process -Id $_.Id -Force
}

# Wait a moment for processes to fully stop
Start-Sleep -Seconds 2

# Now build the API
Write-Host "`n=== BUILDING API ===" -ForegroundColor Green
cd C:\Users\cluiz\code-base-Insurex\InsureX.ModernAPI
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Build successful!" -ForegroundColor Green
    
    # Start the API in a new window
    Write-Host "`nStarting API in new window..." -ForegroundColor Cyan
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd C:\Users\cluiz\code-base-Insurex\InsureX.ModernAPI; dotnet run"
    
    # Wait for API to start
    Start-Sleep -Seconds 5
    
    # Test the API
    Write-Host "`nTesting API..." -ForegroundColor Cyan
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5012/health" -TimeoutSec 2
        Write-Host "✅ API is responding!" -ForegroundColor Green
        Write-Host "   Swagger: http://localhost:5012/swagger"
    } catch {
        Write-Host "⚠️ API not responding yet - check in a moment" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n❌ Build failed!" -ForegroundColor Red
    Write-Host "Check the error messages above" -ForegroundColor Yellow
}

# Start React
Write-Host "`n=== STARTING REACT ===" -ForegroundColor Green
cd C:\Users\cluiz\code-base-Insurex\insurex-react-app

# Check if node_modules exists
if (-not (Test-Path "node_modules")) {
    Write-Host "Installing dependencies..." -ForegroundColor Cyan
    npm install
}

# Start React in a new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd C:\Users\cluiz\code-base-Insurex\insurex-react-app; npm start"

Write-Host "`n=== ALL SYSTEMS STARTED ===" -ForegroundColor Magenta
Write-Host "📍 API: http://localhost:5012"
Write-Host "📍 Swagger: http://localhost:5012/swagger"
Write-Host "📍 React: http://localhost:3000"
Write-Host ""
Write-Host "Press any key to view running processes..."
Read-Host

# Show running processes
Get-Process | Where-Object { $_.ProcessName -like "*dotnet*" -or $_.ProcessName -like "*node*" } | Format-Table Id, ProcessName, CPU -AutoSize
