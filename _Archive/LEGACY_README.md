# 🏛️ Legacy Projects Archive

## Overview
This archive contains the legacy components of the InsureX project that are being phased out as part of the modernization effort.

## Archived Projects

### 📂 Legacy_API/IAPR_API
- **Type**: WCF REST Services
- **Status**: ✅ Fully replaced by `InsureX.ModernAPI`
- **Last Active**: February 2026
- **Key Endpoints**: 
  - addAsset.svc
  - removeAsset.svc
  - policyNonpayment.svc

### 📂 Legacy_Web/IAPR_Web
- **Type**: ASP.NET WebForms
- **Status**: ⚠️ Being replaced by React frontend (65% complete)
- **Last Active**: February 2026
- **Key Pages**:
  - Policies.aspx
  - Assets.aspx
  - AddAsset.aspx (with 11 asset type forms)

### 📂 Legacy_Data/IAPR_Data
- **Type**: Class Library with ADO.NET
- **Status**: ⚠️ Being migrated to EF Core
- **Key Components**:
  - Provider classes (UserProvider, PolicyProvider, etc.)
  - Data encryption utilities

### 📂 Legacy_Services/InsurexService
- **Type**: Windows Service
- **Status**: ⏳ To be replaced by background jobs in modern API

## Migration Notes
- All data has been migrated to the new database schema
- Modern API uses EF Core with the same database
- React frontend will eventually replace all WebForms functionality

## If You Need to Run Legacy Projects
1. These projects require older .NET Framework (4.7.2+)
2. They are kept for reference only
3. No further development should be done on legacy code