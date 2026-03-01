-- Billing Module Tables for InsureX
USE InsureX;
GO

-- Invoices table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Invoices')
BEGIN
    CREATE TABLE Invoices (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        InvoiceNumber NVARCHAR(50) NOT NULL,
        PolicyId INT NOT NULL,
        InvoiceDate DATETIME2 NOT NULL,
        DueDate DATETIME2 NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Tax DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalAmount DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, Sent, Paid, Overdue, Cancelled
        PaymentMethod NVARCHAR(50) NULL,
        PaymentDate DATETIME2 NULL,
        Notes NVARCHAR(MAX) NULL,
        PdfUrl NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        CreatedBy INT NULL,
        FOREIGN KEY (PolicyId) REFERENCES Policies(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
    );

    CREATE INDEX IX_Invoices_PolicyId ON Invoices(PolicyId);
    CREATE INDEX IX_Invoices_Status ON Invoices(Status);
    CREATE INDEX IX_Invoices_DueDate ON Invoices(DueDate);
    PRINT 'Created Invoices table';
END
GO

-- Invoice Items table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'InvoiceItems')
BEGIN
    CREATE TABLE InvoiceItems (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        InvoiceId INT NOT NULL,
        Description NVARCHAR(200) NOT NULL,
        Quantity INT NOT NULL DEFAULT 1,
        UnitPrice DECIMAL(18,2) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        ItemType NVARCHAR(50) NOT NULL, -- Premium, Fee, Tax, Adjustment
        ReferenceId INT NULL, -- Could reference Policy, Claim, etc.
        ReferenceType NVARCHAR(50) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_InvoiceItems_InvoiceId ON InvoiceItems(InvoiceId);
    PRINT 'Created InvoiceItems table';
END
GO

-- Payments table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Payments')
BEGIN
    CREATE TABLE Payments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PaymentNumber NVARCHAR(50) NOT NULL,
        InvoiceId INT NOT NULL,
        PaymentDate DATETIME2 NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        PaymentMethod NVARCHAR(50) NOT NULL, -- Credit Card, Bank Transfer, Cash, Cheque
        Reference NVARCHAR(100) NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Completed, Failed, Refunded
        TransactionId NVARCHAR(100) NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedBy INT NULL,
        FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
    );

    CREATE INDEX IX_Payments_InvoiceId ON Payments(InvoiceId);
    CREATE INDEX IX_Payments_Status ON Payments(Status);
    PRINT 'Created Payments table';
END
GO

-- Recurring Billing table (for automatic monthly invoices)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'RecurringBilling')
BEGIN
    CREATE TABLE RecurringBilling (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PolicyId INT NOT NULL,
        Frequency NVARCHAR(20) NOT NULL, -- Monthly, Quarterly, Annually
        NextBillingDate DATETIME2 NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        LastInvoiceId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        FOREIGN KEY (PolicyId) REFERENCES Policies(Id),
        FOREIGN KEY (LastInvoiceId) REFERENCES Invoices(Id)
    );

    CREATE INDEX IX_RecurringBilling_PolicyId ON RecurringBilling(PolicyId);
    CREATE INDEX IX_RecurringBilling_NextBillingDate ON RecurringBilling(NextBillingDate);
    PRINT 'Created RecurringBilling table';
END
GO

-- Sample data for testing
PRINT 'Adding sample invoice data...';

-- Get a policy ID to use for sample data
DECLARE @PolicyId INT = (SELECT TOP 1 Id FROM Policies ORDER BY Id);
DECLARE @UserId INT = (SELECT TOP 1 Id FROM Users ORDER BY Id);

IF @PolicyId IS NOT NULL AND @UserId IS NOT NULL
BEGIN
    -- Insert sample invoice
    INSERT INTO Invoices (InvoiceNumber, PolicyId, InvoiceDate, DueDate, Amount, Tax, TotalAmount, Status, CreatedBy)
    VALUES ('INV-2026-0001', @PolicyId, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1250.00, 187.50, 1437.50, 'Sent', @UserId);

    DECLARE @InvoiceId INT = SCOPE_IDENTITY();

    -- Insert invoice items
    INSERT INTO InvoiceItems (InvoiceId, Description, Quantity, UnitPrice, Amount, ItemType)
    VALUES 
        (@InvoiceId, 'Monthly Premium - Vehicle Insurance', 1, 950.00, 950.00, 'Premium'),
        (@InvoiceId, 'Administration Fee', 1, 300.00, 300.00, 'Fee'),
        (@InvoiceId, 'VAT @ 15%', 1, 187.50, 187.50, 'Tax');

    PRINT 'Added sample invoice data';
END
GO

-- Display table counts
SELECT 'Invoices' as TableName, COUNT(*) as Count FROM Invoices
UNION ALL
SELECT 'InvoiceItems', COUNT(*) FROM InvoiceItems
UNION ALL
SELECT 'Payments', COUNT(*) FROM Payments
UNION ALL
SELECT 'RecurringBilling', COUNT(*) FROM RecurringBilling;
GO
