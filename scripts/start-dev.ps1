# start-dev.ps1
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Iniciando InsureX Development Environment" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Iniciar Modern API em background
Write-Host ""
Write-Host "1. Iniciando Modern API..." -ForegroundColor Green
$apiJob = Start-Job -ScriptBlock {
    Set-Location "C:\Users\cluiz\code-base-Insurex\InsureX.ModernAPI"
    dotnet run
}

Write-Host "   API iniciada em: http://localhost:5012" -ForegroundColor Yellow
Write-Host "   Swagger: http://localhost:5012/swagger" -ForegroundColor Yellow

# Aguardar API iniciar
Start-Sleep -Seconds 5

# Iniciar React App
Write-Host ""
Write-Host "2. Iniciando React App..." -ForegroundColor Green
Set-Location "C:\Users\cluiz\code-base-Insurex\insurex-react-app"

# Verificar se node_modules existe
if (!(Test-Path "node_modules")) {
    Write-Host "   Instalando dependências..." -ForegroundColor Yellow
    npm install
}

# Iniciar React
Start-Process npm -ArgumentList "start"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "✅ Ambiente iniciado!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "📱 React App: http://localhost:3000" -ForegroundColor Cyan
Write-Host "🔧 API: http://localhost:5012" -ForegroundColor Cyan
Write-Host "📚 Swagger: http://localhost:5012/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Pressione Ctrl+C para encerrar"