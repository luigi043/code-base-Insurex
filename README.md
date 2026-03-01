
# 🏢 InsureX - Insured Asset Protection Register (IAPR)

[![Build Status](https://github.com/luigi043/code-base-Insurex/actions/workflows/dotnet.yml/badge.svg)](https://github.com/luigi043/code-base-Insurex/actions/workflows/dotnet.yml)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![React Version](https://img.shields.io/badge/React-18.x-61DAFB)](https://reactjs.org/)
[![License](https://img.shields.io/badge/License-Internal-blue)](LICENSE)

## 📋 Project Overview

InsureX is a comprehensive **Insured Asset Protection Register (IAPR)** system designed to manage insurance policies, assets, claims, and partner relationships. The system handles 11 different asset types including vehicles, property, watercraft, aviation, and more.

### 🏗️ Architecture

The project is currently in **active modernization phase**, transitioning from legacy technologies to modern stack:

| Component | Legacy | Modern | Status |
|-----------|--------|--------|--------|
| **Backend API** | WCF REST Services | .NET 8 Web API | ✅ 100% Complete |
| **Authentication** | Basic Auth | JWT + BCrypt | ✅ 100% Complete |
| **Frontend** | ASP.NET WebForms | React 18 SPA | ⚠️ 65% Complete |
| **Data Access** | Stored Procedures | EF Core + SQL | ✅ 100% Complete |
| **Documentation** | None | Swagger/OpenAPI | ✅ 100% Complete |
| **Testing** | None | xUnit + Integration Tests | ⚠️ 25% Complete |
| **DevOps** | Manual | GitHub Actions + Docker | ⚠️ 40% Complete |

## ✨ Key Features

### ✅ Completed (85%)
- **Modern .NET 8 Web API** with RESTful endpoints
- **JWT Authentication** with secure token management
- **11 Asset Types** fully implemented:
  - Vehicle, Property, Watercraft, Aviation
  - Stock/Inventory, Accounts Receivable
  - Machinery, Plant & Equipment
  - Business Interruption, Keyman Insurance
  - Electronic Equipment
- **Policy Management** (CRUD operations)
- **Claims Management** (API ready)
- **SQL Server Database** with EF Core
- **Swagger/OpenAPI** documentation at `/swagger`
- **Integration Tests** project with passing tests
- **CI/CD Pipeline** with GitHub Actions
- **Docker Support** with docker-compose

### 🚧 In Progress (15%)
- React Frontend (65% complete)
- Claims approval workflow
- Billing module
- Advanced reporting

## 🛠️ Technology Stack

### Backend
| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM for database access |
| SQL Server | 2022 | Primary database |
| JWT Bearer | 8.0 | Authentication |
| BCrypt | Next | Password hashing |
| Swashbuckle | 6.5 | Swagger/OpenAPI |
| xUnit | 2.7 | Testing framework |

### Frontend
| Technology | Version | Purpose |
|------------|---------|---------|
| React | 18.x | UI library |
| React Router | 6.x | Navigation |
| Axios | 1.x | API communication |
| CSS/SCSS | - | Styling |

### DevOps
| Technology | Purpose |
|------------|---------|
| GitHub Actions | CI/CD pipeline |
| Docker | Containerization |
| Docker Compose | Multi-container orchestration |

## 📁 Project Structure

```
code-base-Insurex/
├── 📂 InsureX.ModernAPI/           # Modern .NET 8 Web API
│   ├── 📂 Controllers/              # API endpoints
│   ├── 📂 Data/                      # DbContext and migrations
│   ├── 📂 Models/                    # Entity classes
│   │   ├── AssetTypes.cs             # 11 asset type classes
│   │   ├── InsuranceClaim.cs         # Claims (renamed from Claim)
│   │   ├── Policy.cs                  # Policy entity
│   │   └── User.cs                    # User entity
│   ├── 📂 Helpers/                    # Utility classes
│   ├── Program.cs                      # Application entry point
│   └── appsettings.json                # Configuration
│
├── 📂 insurex-react-app/             # React Frontend (65%)
│   ├── 📂 src/
│   │   ├── 📂 components/            # React components
│   │   ├── 📂 services/               # API services
│   │   └── App.js                     # Main app component
│   └── package.json                    # npm dependencies
│
├── 📂 InsureX.IntegrationTests/      # Integration Tests
│   ├── UnitTest1.cs                    # Initial passing test
│   └── InsureX.IntegrationTests.csproj
│
├── 📂 Database/                       # Database scripts
│   └── 📂 Scripts/
│       ├── 03 - Seed Data.sql          # Test data (3 policies, 3 assets)
│       └── 06 - Indexes.sql            # Performance indexes
│
├── 📂 IAPR_*/                         # Legacy projects (archiving)
├── 📂 _Archive/                        # Archived legacy projects
│
├── 📂 .github/
│   └── 📂 workflows/
│       └── dotnet.yml                  # GitHub Actions CI/CD
│
├── docker-compose.yml                   # Docker configuration
├── test-all.ps1                         # End-to-end test script
├── check-status.ps1                      # Project status checker
├── MASTER_CHECKLIST.md                   # Consolidated project checklist
└── README.md                             # This file
```

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server 2022](https://www.microsoft.com/sql-server) (or LocalDB)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional)
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)

### Quick Start (5 minutes)

```powershell
# 1. Clone the repository
git clone https://github.com/luigi043/code-base-Insurex.git
cd code-base-Insurex

# 2. Start the API
cd InsureX.ModernAPI
dotnet restore
dotnet run
# API will be available at https://localhost:5012

# 3. In a new terminal, start React frontend
cd insurex-react-app
npm install
npm start
# React app will open at http://localhost:3000

# 4. Test the API
cd ..
.\test-all.ps1
```

### Database Setup

The API will automatically create the database on first run. To add seed data and indexes:

```powershell
# Run database setup script
.\setup-database.ps1
```

This will:
- Create all tables via EF Core migrations
- Add performance indexes
- Seed test data (3 policies, 3 assets, 1 admin user)

## 📡 API Documentation

Once the API is running, access Swagger UI at:
```
https://localhost:5012/swagger
```

### Available Endpoints

| Category | Endpoints | Description |
|----------|-----------|-------------|
| **Auth** | `POST /api/auth/register`<br>`POST /api/auth/login` | User registration and JWT login |
| **Policies** | `GET /api/policies`<br>`GET /api/policies/{id}`<br>`POST /api/policies`<br>`PUT /api/policies/{id}`<br>`DELETE /api/policies/{id}` | Full CRUD operations |
| **Assets** | `GET /api/assets`<br>`GET /api/assets/{id}`<br>`POST /api/assets` | Asset management for 11 types |
| **Claims** | `GET /api/claims`<br>`GET /api/claims/{id}`<br>`POST /api/claims`<br>`PUT /api/claims/{id}` | Claims processing |
| **Health** | `GET /health` | API health check |

### Authentication Example

```powershell
# Get JWT token
$loginBody = @{
    email = "admin@insurex.com"
    password = "password"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:5012/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body $loginBody

$token = $response.token

# Use token for authenticated requests
$policies = Invoke-RestMethod -Uri "https://localhost:5012/api/policies" `
    -Headers @{Authorization = "Bearer $token"}
```

## 🧪 Testing

### Run All Tests
```powershell
# Unit + Integration Tests
dotnet test
```

### Integration Tests
```powershell
cd InsureX.IntegrationTests
dotnet test
```

### Manual API Testing
```powershell
.\test-all.ps1
```

## 🐳 Docker Deployment

```powershell
# Make sure Docker Desktop is running
docker-compose up --build
```

This starts:
- API container on port 5012
- SQL Server container on port 1433

## 🔧 Configuration

### User Secrets (Development)
```powershell
cd InsureX.ModernAPI
dotnet user-secrets init
dotnet user-secrets set "JWT:Secret" "your-32-char-secret-key-here!"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=InsureX;Trusted_Connection=True;TrustServerCertificate=true"
```

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JWT": {
    "Secret": "",  // Stored in user secrets
    "ExpirationInMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": ""  // Stored in user secrets
  }
}
```

## 📊 Project Status Dashboard

Run the status checker to see real-time project health:
```powershell
.\check-status.ps1
```

Current overall completion: **85%**

| Module | Status | Completion |
|--------|--------|------------|
| Modern API | ✅ Complete | 100% |
| Database | ✅ Complete | 100% |
| Authentication | ✅ Complete | 100% |
| Asset Types | ✅ Complete | 100% |
| React Frontend | ⚠️ In Progress | 65% |
| Testing | ⚠️ Started | 25% |
| DevOps | ⚠️ Started | 40% |
| Documentation | ⚠️ Partial | 60% |

## 🤝 Contributing

1. Create a feature branch from `main`
2. Make your changes
3. Run tests: `dotnet test`
4. Submit a Pull Request

### Coding Standards
- Follow C# coding conventions
- Add XML comments for public APIs
- Write tests for new features
- Update documentation as needed

## 🐛 Known Issues

| ID | Issue | Status | Target Fix |
|----|-------|--------|------------|
| IAPR-002 | SMTP credentials exposed | ⚠️ In Progress | Week 3-4 |
| IAPR-005 | Low test coverage (8%) | ⚠️ In Progress | Week 3-4 |
| IAPR-010 | Legacy build warnings | ⚠️ Ongoing | Legacy only |
| N/A | Docker connection on Windows | ⚠️ In Progress | Week 5-6 |

## 📅 Roadmap

### Q2 2026
- ✅ Complete Modern API (Done)
- ✅ Add Swagger documentation (Done)
- ✅ Set up CI/CD (Done)
- 🔄 Complete React frontend (Week 3-4)
- 🔄 Achieve 30% test coverage (Week 3-4)
- 🔄 Deploy to staging (Week 5-6)

### Q3 2026
- Complete migration from legacy API
- Add advanced reporting
- Implement document upload
- Mobile app development

## 📚 Additional Documentation

- [Master Checklist](MASTER_CHECKLIST.md) - Detailed project progress
- [API Endpoints](InsureX.ModernAPI/Controllers/) - Source code documentation
- [Database Schema](Database/Scripts/) - SQL scripts

## 👥 Team

- **Luiz Fehlberg** - Lead Developer ([@luigi043](https://github.com/luigi043))

## 📄 License

Internal Use Only - All Rights Reserved

---

**Last Updated:** 2026-03-01
**Current Version:** 1.0.0
**Status:** 🟢 Active Development - 85% Complete

For questions or support, contact the development team.