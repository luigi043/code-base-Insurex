Write-Host "=== COMPLETE INSUREX SETUP ===" -ForegroundColor Magenta

# 1. Fix Integration Tests
Write-Host "`n1. Fixing Integration Tests..." -ForegroundColor Cyan
cd InsureX.IntegrationTests

# Update packages
dotnet remove package Microsoft.EntityFrameworkCore
dotnet remove package Microsoft.EntityFrameworkCore.InMemory
dotnet remove package Microsoft.EntityFrameworkCore.SqlServer
dotnet remove package Microsoft.AspNetCore.Mvc.Testing

dotnet add package Microsoft.EntityFrameworkCore --version 8.0.3
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 8.0.3
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.3
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.3
dotnet add package Microsoft.NET.Test.Sdk --version 17.8.0
dotnet add package xunit --version 2.6.2
dotnet add package xunit.runner.visualstudio --version 2.5.4

# Run tests
Write-Host "`nRunning tests..." -ForegroundColor Yellow
dotnet test

# 2. Set up Vite React app
Write-Host "`n2. Setting up Vite React app..." -ForegroundColor Cyan
cd ..

if (Test-Path "insurex-react-vite") {
    Remove-Item -Recurse -Force insurex-react-vite -ErrorAction SilentlyContinue
}

npm create vite@latest insurex-react-vite -- --template react
cd insurex-react-vite
npm install
npm install axios react-router-dom recharts react-datepicker formik yup

Write-Host "`n3. Starting Vite dev server..." -ForegroundColor Green
Write-Host "Press Ctrl+C to stop the server"
npm run dev
