-- 06 - Indexes.sql
-- Performance optimization indexes for InsureX database

USE [InsureX];
GO

-- Indexes for Policies table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Policies_PolicyNumber')
CREATE UNIQUE NONCLUSTERED INDEX IX_Policies_PolicyNumber ON Policies(PolicyNumber);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Policies_Status')
CREATE NONCLUSTERED INDEX IX_Policies_Status ON Policies(Status) INCLUDE (PolicyNumber, PolicyHolder);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Policies_StartDate_EndDate')
CREATE NONCLUSTERED INDEX IX_Policies_StartDate_EndDate ON Policies(StartDate, EndDate) INCLUDE (PolicyNumber, Status);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Policies_PartnerId')
CREATE NONCLUSTERED INDEX IX_Policies_PartnerId ON Policies(PartnerId) WHERE PartnerId IS NOT NULL;
GO

-- Indexes for Assets table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_PolicyId')
CREATE NONCLUSTERED INDEX IX_Assets_PolicyId ON Assets(PolicyId) INCLUDE (AssetType, Description, InsuredValue);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_AssetType')
CREATE NONCLUSTERED INDEX IX_Assets_AssetType ON Assets(AssetType) INCLUDE (PolicyId, Status);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_Status')
CREATE NONCLUSTERED INDEX IX_Assets_Status ON Assets(Status) INCLUDE (PolicyId, AssetType);
GO

-- Indexes for Claims table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Claims_ClaimNumber')
CREATE UNIQUE NONCLUSTERED INDEX IX_Claims_ClaimNumber ON Claims(ClaimNumber);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Claims_PolicyId')
CREATE NONCLUSTERED INDEX IX_Claims_PolicyId ON Claims(PolicyId) INCLUDE (Status, ClaimAmount);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Claims_Status')
CREATE NONCLUSTERED INDEX IX_Claims_Status ON Claims(Status) INCLUDE (PolicyId, ClaimDate);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Claims_ClaimDate')
CREATE NONCLUSTERED INDEX IX_Claims_ClaimDate ON Claims(ClaimDate) INCLUDE (PolicyId, Status);
GO

-- Indexes for Transactions table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_TransactionNumber')
CREATE UNIQUE NONCLUSTERED INDEX IX_Transactions_TransactionNumber ON Transactions(TransactionNumber);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_PolicyId')
CREATE NONCLUSTERED INDEX IX_Transactions_PolicyId ON Transactions(PolicyId) INCLUDE (TransactionDate, Amount, TransactionType);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_TransactionDate')
CREATE NONCLUSTERED INDEX IX_Transactions_TransactionDate ON Transactions(TransactionDate) INCLUDE (PolicyId, TransactionType);
GO

-- Indexes for Users table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
GO

-- Update statistics
UPDATE STATISTICS Policies;
UPDATE STATISTICS Assets;
UPDATE STATISTICS Claims;
UPDATE STATISTICS Transactions;
UPDATE STATISTICS Users;
GO

PRINT 'Indexes created successfully';
GO
