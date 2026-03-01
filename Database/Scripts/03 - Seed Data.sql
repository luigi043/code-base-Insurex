-- 03 - Seed Data.sql
-- Insert test users
USE [InsureX];
GO

-- Insert admin user if not exists
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@insurex.com')
BEGIN
    INSERT INTO Users (Name, Email, PasswordHash, Role, CreatedAt, IsActive)
    VALUES ('Admin User', 'admin@insurex.com', '', 'Admin', GETDATE(), 1);
END

-- Insert test policies
IF NOT EXISTS (SELECT * FROM Policies WHERE PolicyNumber = 'POL-2025-0001')
BEGIN
    INSERT INTO Policies (PolicyNumber, PolicyHolder, Email, Phone, StartDate, EndDate, Status, Premium, PolicyType, CreatedAt, IsDeleted)
    VALUES 
    ('POL-2025-0001', 'John Smith', 'john.smith@email.com', '555-0101', DATEADD(month, -6, GETDATE()), DATEADD(month, 6, GETDATE()), 'Active', 1200.00, 'Personal', GETDATE(), 0),
    ('POL-2025-0002', 'Sarah Johnson', 'sarah.j@email.com', '555-0102', DATEADD(month, -3, GETDATE()), DATEADD(month, 9, GETDATE()), 'Active', 2500.00, 'Business', GETDATE(), 0),
    ('POL-2025-0003', 'Mike Wilson', 'mike.w@email.com', '555-0103', DATEADD(month, -9, GETDATE()), DATEADD(month, 3, GETDATE()), 'Active', 800.00, 'Personal', GETDATE(), 0);
END

-- Insert test assets
IF NOT EXISTS (SELECT * FROM Assets WHERE Description = '2023 Toyota Camry')
BEGIN
    INSERT INTO Assets (AssetType, Description, PolicyId, FinanceValue, InsuredValue, Status, JsonData, CreatedAt, IsDeleted)
    VALUES 
    ('Vehicle', '2023 Toyota Camry', 1, 25000.00, 28000.00, 'Active', '{"make":"Toyota","model":"Camry","year":2023,"vin":"ABC123XYZ"}', GETDATE(), 0),
    ('Property', 'Main Office Building', 2, 350000.00, 400000.00, 'Active', '{"address":"123 Business Ave","city":"Metropolis","size":5000}', GETDATE(), 0),
    ('Vehicle', '2022 Honda Civic', 3, 18000.00, 20000.00, 'Active', '{"make":"Honda","model":"Civic","year":2022,"vin":"DEF456UVW"}', GETDATE(), 0);
END

PRINT 'Seed data inserted successfully';
GO

-- Add to 03 - Seed Data.sql
INSERT INTO Policies (PolicyNumber, CustomerName, StartDate, EndDate, Status, TotalInsuredValue)
VALUES 
('POL-2026-001', 'ABC Corporation', '2026-01-01', '2027-01-01', 'Active', 5000000),
('POL-2026-002', 'XYZ Enterprises', '2026-02-15', '2027-02-15', 'Active', 2500000),
('POL-2026-003', 'Smith Family', '2026-03-01', '2027-03-01', 'Pending', 750000);

-- Admin, Financer, Insurer, Customer roles
INSERT INTO Users (Email, PasswordHash, Role, PartnerId)
VALUES 
('admin@insurex.com', 'hashed_pwd', 'Admin', NULL),
('financer@bank.com', 'hashed_pwd', 'Financer', 1),
('insurer@mutual.com', 'hashed_pwd', 'Insurer', 2);