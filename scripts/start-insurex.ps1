Write-Host "=== STARTING INSUREX ===" -ForegroundColor Cyan

# Start API in new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd InsureX.ModernAPI; dotnet run"

Write-Host "✅ API starting at http://localhost:5012"

# Wait for API to initialize
Start-Sleep -Seconds 3

# Check if React exists and start it
if (Test-Path "insurex-react-app") {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd insurex-react-app; npm start"
    Write-Host "✅ React starting at http://localhost:3000"
} elseif (Test-Path "insurex-react-vite") {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd insurex-react-vite; npm run dev"
    Write-Host "✅ React (Vite) starting at http://localhost:5173"
}

Write-Host "`n📊 Open these URLs:"
Write-Host "   API Swagger: http://localhost:5012/swagger"
Write-Host "   React App: http://localhost:3000 (or :5173 for Vite)"

Write-Host "`n🧪 Run tests in another terminal:"
Write-Host "   cd InsureX.IntegrationTests; dotnet test"
