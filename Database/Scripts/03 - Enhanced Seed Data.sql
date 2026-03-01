-- ============================================
-- INSUREX ENHANCED SEED DATA
-- ============================================
USE [InsureX]
GO

-- 1. Add More Partners
INSERT INTO Partners (Name, Type, Email, Phone, IsActive)
VALUES 
('First National Bank', 'Financer', 'partners@fnb.co.za', '+27112345678', 1),
('Standard Insurance', 'Insurer', 'claims@standard.co.za', '+27119876543', 1),
('ABSA Group', 'Financer', 'partners@absa.co.za', '+27115556666', 1),
('Old Mutual', 'Insurer', 'support@oldmutual.co.za', '+27112223333', 1),
('Nedbank', 'Financer', 'partners@nedbank.co.za', '+27114447777', 1);
GO

-- 2. Add More Users with Different Roles
INSERT INTO Users (Email, PasswordHash, Role, PartnerId, IsActive)
VALUES 
-- Admins
('admin@insurex.com', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Admin', NULL, 1),
('system@insurex.com', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Admin', NULL, 1),

-- Financer Users
('john@fnb.co.za', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Financer', 1, 1),
('sarah@absa.co.za', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Financer', 3, 1),
('peter@nedbank.co.za', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Financer', 5, 1),

-- Insurer Users
('claims@standard.co.za', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Insurer', 2, 1),
('underwriting@oldmutual.co.za', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Insurer', 4, 1),

-- Customer Users
('mike.brown@gmail.com', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Customer', NULL, 1),
('lisa.white@yahoo.com', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Customer', NULL, 1),
('robert.johnson@gmail.com', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Customer', NULL, 1);
GO

-- 3. Add More Policies
INSERT INTO Policies (PolicyNumber, CustomerName, CustomerEmail, StartDate, EndDate, Status, TotalInsuredValue, PartnerId, CreatedBy)
VALUES 
-- Active Policies
('POL-2026-001', 'ABC Corporation', 'finance@abc.co.za', '2026-01-01', '2027-01-01', 'Active', 5000000.00, 1, 1),
('POL-2026-002', 'XYZ Enterprises', 'accounts@xyz.co.za', '2026-02-15', '2027-02-15', 'Active', 2500000.00, 3, 1),
('POL-2026-003', 'Smith Family', 'smith.family@gmail.com', '2026-03-01', '2027-03-01', 'Active', 750000.00, 5, 1),
('POL-2026-004', 'Johnson Transport', 'admin@johntrans.co.za', '2026-01-15', '2027-01-15', 'Active', 3500000.00, 1, 1),
('POL-2026-005', 'Cape Industries', 'finance@capeind.co.za', '2026-02-01', '2027-02-01', 'Active', 4200000.00, 3, 1),

-- Policies Expiring Soon
('POL-2025-099', 'Durban Logistics', 'accounts@durbanlog.co.za', '2025-03-15', '2026-03-15', 'Active', 2800000.00, 2, 1),
('POL-2025-100', 'Port Elizabeth Manufacturing', 'finance@pem.co.za', '2025-03-20', '2026-03-20', 'Active', 3100000.00, 4, 1),

-- Suspended Policies
('POL-2025-050', 'Troubled Company', 'info@troubled.co.za', '2025-06-01', '2026-06-01', 'Suspended', 1500000.00, 1, 1),

-- Cancelled Policies
('POL-2025-025', 'Closed Business', 'owner@closed.co.za', '2025-01-01', '2025-12-31', 'Cancelled', 500000.00, 3, 1),

-- Pending Policies
('POL-2026-010', 'New Customer', 'new.customer@gmail.com', '2026-03-10', '2027-03-10', 'Pending', 1200000.00, 2, 1);
GO

-- 4. Add More Assets (Sample for Vehicle type - repeat for other types)
-- First, get Policy IDs
DECLARE @Policy1 INT = (SELECT Id FROM Policies WHERE PolicyNumber = 'POL-2026-001')
DECLARE @Policy2 INT = (SELECT Id FROM Policies WHERE PolicyNumber = 'POL-2026-002')
DECLARE @Policy3 INT = (SELECT Id FROM Policies WHERE PolicyNumber = 'POL-2026-003')
DECLARE @Policy4 INT = (SELECT Id FROM Policies WHERE PolicyNumber = 'POL-2026-004')
DECLARE @Policy5 INT = (SELECT Id FROM Policies WHERE PolicyNumber = 'POL-2026-005')

-- Vehicle Assets
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status)
VALUES 
(@Policy1, 'Vehicle', '{"Make":"Toyota","Model":"Hilux","Year":2023,"VIN":"ABC123XYZ789","Registration":"CA123456","Color":"White"}', 450000.00, 500000.00, 'Active'),
(@Policy1, 'Vehicle', '{"Make":"Ford","Model":"Ranger","Year":2022,"VIN":"DEF456UVW012","Registration":"CA789012","Color":"Blue"}', 380000.00, 420000.00, 'Active'),
(@Policy2, 'Vehicle', '{"Make":"Volkswagen","Model":"Amarok","Year":2023,"VIN":"GHI789RST345","Registration":"CY345678","Color":"Black"}', 520000.00, 550000.00, 'Active'),
(@Policy3, 'Vehicle', '{"Make":"Toyota","Model":"Corolla","Year":2021,"VIN":"JKL012MNO678","Registration":"CA901234","Color":"Silver"}', 180000.00, 200000.00, 'Active'),
(@Policy4, 'Vehicle', '{"Make":"Mercedes","Model":"Sprinter","Year":2022,"VIN":"PQR345STU901","Registration":"CA567890","Color":"White"}', 350000.00, 380000.00, 'Active'),

-- Property Assets
(@Policy1, 'Property', '{"Address":"123 Main St","City":"Johannesburg","ERF":"1234","Size":500,"Type":"Commercial"}', 2500000.00, 2750000.00, 'Active'),
(@Policy2, 'Property', '{"Address":"456 Oak Ave","City":"Cape Town","ERF":"5678","Size":350,"Type":"Retail"}', 1800000.00, 1950000.00, 'Active'),
(@Policy5, 'Property', '{"Address":"789 Pine Rd","City":"Durban","ERF":"9012","Size":450,"Type":"Industrial"}', 2200000.00, 2400000.00, 'Active'),

-- Watercraft Assets
(@Policy4, 'Watercraft', '{"Type":"Yacht","Length":12.5,"Year":2022,"Make":"Bayliner","Model":"185","HullNumber":"BLN12345D686"}', 350000.00, 380000.00, 'Active'),
(@Policy5, 'Watercraft', '{"Type":"Fishing Boat","Length":8.2,"Year":2021,"Make":"Duckworth","Model":"Patriot","HullNumber":"DUK78901A234"}', 180000.00, 200000.00, 'Active'),

-- Electronic Equipment
(@Policy2, 'ElectronicEquipment', '{"Type":"Server","Make":"Dell","Model":"PowerEdge","SerialNumber":"DELL123456","Warranty":true}', 45000.00, 50000.00, 'Active'),
(@Policy3, 'ElectronicEquipment', '{"Type":"Security System","Make":"Hikvision","Model":"DS-2CD","SerialNumber":"HIK789012","Warranty":true}', 15000.00, 18000.00, 'Active');
GO

-- 5. Add Sample Claims
INSERT INTO Claims (PolicyId, ClaimNumber, DateOfLoss, ClaimAmount, Status, Description, FiledBy)
VALUES 
(@Policy1, 'CLM-2026-001', '2026-02-15', 25000.00, 'Submitted', 'Vehicle accident damage', 1),
(@Policy2, 'CLM-2026-002', '2026-02-20', 15000.00, 'In Review', 'Burglary at retail store', 1),
(@Policy3, 'CLM-2026-003', '2026-02-25', 5000.00, 'Approved', 'Electronic equipment failure', 1),
(@Policy4, 'CLM-2026-004', '2026-03-01', 35000.00, 'Submitted', 'Boat engine damage', 1),
(@Policy5, 'CLM-2026-005', '2026-03-02', 7500.00, 'In Review', 'Theft of equipment', 1);
GO

-- 6. Add Transactions
INSERT INTO Transactions (PolicyId, TransactionDate, Type, Amount, Description, Status)
VALUES 
(@Policy1, '2026-01-01', 'Premium', 2500.00, 'Monthly premium', 'Paid'),
(@Policy1, '2026-02-01', 'Premium', 2500.00, 'Monthly premium', 'Paid'),
(@Policy1, '2026-03-01', 'Premium', 2500.00, 'Monthly premium', 'Pending'),
(@Policy2, '2026-02-15', 'Premium', 1800.00, 'Monthly premium', 'Paid'),
(@Policy3, '2026-03-01', 'Premium', 950.00, 'Monthly premium', 'Pending'),
(@Policy4, '2026-01-15', 'Premium', 2200.00, 'Monthly premium', 'Paid'),
(@Policy5, '2026-02-01', 'Premium', 2800.00, 'Monthly premium', 'Paid');
GO

PRINT '✅ Enhanced seed data added successfully!'