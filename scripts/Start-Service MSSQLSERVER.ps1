# 1. Start SQL Server
Start-Service MSSQLSERVER

# 2. Create database and seed data
sqlcmd -S localhost -Q "CREATE DATABASE InsureX"
.\setup-database.ps1

# 3. Update vulnerable packages
cd InsureX.IntegrationTests
dotnet add package Azure.Identity --version 1.11.0
dotnet add package Microsoft.Data.SqlClient --version 5.2.0
dotnet add package Microsoft.Extensions.Caching.Memory --version 8.0.1
cd ..

# 4. Verify everything works
.\check-status-fixed.ps1

# 5. Run all tests
dotnet test
.\test-all.ps1