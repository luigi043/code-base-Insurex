-- ============================================
-- INSUREX - COMPREHENSIVE SEED DATA
-- ============================================

USE [InsureX];
GO

-- 1. PARTNERS (Financers & Insurers)
INSERT INTO Partners (Name, Type, ContactEmail, ContactPhone, IsActive)
VALUES 
('First National Bank', 'Financer', 'partners@fnb.co.za', '+27 11 123 4567', 1),
('Standard Insurance Ltd', 'Insurer', 'corporate@standardinsure.com', '+27 21 987 6543', 1),
('ABSA Financial Services', 'Financer', 'partners@absa.co.za', '+27 31 456 7890', 1),
('Old Mutual Insure', 'Insurer', 'partnerships@oldmutual.co.za', '+27 21 555 1234', 1),
('Nedbank Insurance', 'Both', 'insurance@nedbank.co.za', '+27 11 234 5678', 1);
GO

-- 2. USERS with different roles
INSERT INTO Users (Email, PasswordHash, Role, PartnerId, FirstName, LastName, IsActive)
VALUES 
-- Admin user (password: Admin@123)
('admin@insurex.com', '$2a$11$YourHashedPasswordHere', 'Admin', NULL, 'System', 'Administrator', 1),

-- Partner users
('john@fnb.co.za', '$2a$11$YourHashedPasswordHere', 'Financer', 1, 'John', 'Smith', 1),
('sarah@standardinsure.com', '$2a$11$YourHashedPasswordHere', 'Insurer', 2, 'Sarah', 'Jones', 1),
('mike@absa.co.za', '$2a$11$YourHashedPasswordHere', 'Financer', 3, 'Mike', 'Peters', 1),

-- Customer users
('customer1@gmail.com', '$2a$11$YourHashedPasswordHere', 'Customer', NULL, 'Peter', 'Johnson', 1),
('customer2@gmail.com', '$2a$11$YourHashedPasswordHere', 'Customer', NULL, 'Mary', 'Williams', 1);
GO

-- 3. POLICIES (15 policies with varied statuses)
INSERT INTO Policies (PolicyNumber, CustomerName, CustomerEmail, StartDate, EndDate, Status, TotalInsuredValue, PartnerId, CreatedBy)
VALUES 
-- Active policies
('POL-2026-001', 'ABC Corporation', 'finance@abccorp.com', '2026-01-01', '2027-01-01', 'Active', 5000000.00, 1, 1),
('POL-2026-002', 'XYZ Enterprises', 'accounts@xyz.co.za', '2026-02-15', '2027-02-15', 'Active', 2500000.00, 3, 1),
('POL-2026-003', 'Smith Family Trust', 'smith.trust@gmail.com', '2026-03-01', '2027-03-01', 'Active', 1750000.00, NULL, 1),
('POL-2026-004', 'Johnson Trading', 'info@johnsontrading.co.za', '2026-01-15', '2027-01-15', 'Active', 3200000.00, 2, 1),
('POL-2026-005', 'Williams & Sons', 'contact@williams.co.za', '2026-02-01', '2027-02-01', 'Active', 4100000.00, 1, 1),

-- Expiring soon (within 30 days)
('POL-2026-006', 'Brown Construction', 'accounts@brownconst.co.za', '2025-03-15', '2026-04-15', 'Active', 8750000.00, 2, 1),
('POL-2026-007', 'Davis Logistics', 'finance@davislogistics.co.za', '2025-04-01', '2026-04-01', 'Active', 6200000.00, 3, 1),

-- Pending policies
('POL-2026-008', 'Miller Farms', 'admin@millerfarms.co.za', '2026-03-10', '2027-03-10', 'Pending', 2300000.00, NULL, 1),
('POL-2026-009', 'Wilson Retail', 'accounts@wilsonretail.co.za', '2026-03-12', '2027-03-12', 'Pending', 1850000.00, 1, 1),

-- Suspended/Cancelled
('POL-2026-010', 'Taylor Motors', 'finance@taylormotors.co.za', '2025-06-01', '2026-06-01', 'Suspended', 5500000.00, 2, 1),
('POL-2026-011', 'Anderson Properties', 'info@andersonprops.co.za', '2025-07-15', '2026-07-15', 'Cancelled', 12000000.00, 3, 1),

-- Expired policies
('POL-2026-012', 'Thomas Transport', 'accounts@thomastransport.co.za', '2025-01-01', '2026-01-01', 'Expired', 3800000.00, 1, 1),
('POL-2026-013', 'Jackson Mining', 'finance@jacksonmining.co.za', '2025-02-15', '2026-02-15', 'Expired', 15000000.00, 2, 1),

-- Additional active policies
('POL-2026-014', 'White Industries', 'info@whiteind.co.za', '2026-01-20', '2027-01-20', 'Active', 7200000.00, NULL, 1),
('POL-2026-015', 'Harris Security', 'accounts@harrissecurity.co.za', '2026-02-28', '2027-02-28', 'Active', 2950000.00, 3, 1);
GO

-- 4. ASSETS (30+ assets across all 11 types)
-- Note: AssetData is JSON column storing type-specific details

-- Vehicle Assets (5)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(1, 'Vehicle', '{"Make":"Toyota","Model":"Hilux","Year":2023,"VIN":"AHTZZZ69L12345678","Registration":"CA 123-456","Color":"White","EngineNumber":"2GD123456"}', 450000.00, 500000.00, 'Active', 1),
(1, 'Vehicle', '{"Make":"Ford","Model":"Ranger","Year":2024,"VIN":"1FTER4FH3LLA12345","Registration":"CA 789-012","Color":"Blue","EngineNumber":"P4AT12345"}', 520000.00, 550000.00, 'Active', 1),
(2, 'Vehicle', '{"Make":"BMW","Model":"X5","Year":2023,"VIN":"5UXCR6C5XL9A12345","Registration":"CA 345-678","Color":"Black","EngineNumber":"B58B12345"}', 850000.00, 900000.00, 'Active', 1),
(3, 'Vehicle', '{"Make":"Mercedes","Model":"C-Class","Year":2022,"VIN":"WDD2050421R123456","Registration":"CY 123-456","Color":"Silver","EngineNumber":"M27412345"}', 550000.00, 600000.00, 'Active', 1),
(4, 'Vehicle', '{"Make":"Volkswagen","Model":"Amarok","Year":2023,"VIN":"WV1ZZZ2HZPA123456","Registration":"CA 901-234","Color":"Grey","EngineNumber":"DDAA12345"}', 580000.00, 620000.00, 'Active', 1);

-- Property Assets (4)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(1, 'Property', '{"Address":"123 Main Street","City":"Johannesburg","Province":"Gauteng","PostalCode":"2001","ERF":"ERF12345","SectionalTitle":"ST001","Size":450,"YearBuilt":2018}', 2500000.00, 2800000.00, 'Active', 1),
(3, 'Property', '{"Address":"45 Beach Road","City":"Cape Town","Province":"Western Cape","PostalCode":"8001","ERF":"ERF67890","Size":320,"YearBuilt":2020}', 3800000.00, 4200000.00, 'Active', 1),
(5, 'Property', '{"Address":"78 Industrial Park","City":"Durban","Province":"KZN","PostalCode":"4001","ERF":"ERF24680","Size":850,"YearBuilt":2015}', 5200000.00, 5800000.00, 'Active', 1),
(7, 'Property', '{"Address":"12 Mountain View","City":"Pretoria","Province":"Gauteng","PostalCode":"0186","ERF":"ERF13579","Size":280,"YearBuilt":2022}', 1950000.00, 2200000.00, 'Active', 1);

-- Watercraft Assets (3)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(2, 'Watercraft', '{"VesselType":"Motor Yacht","Make":"Bayliner","Model":"185","Length":18,"Year":2022,"HullNumber":"BLN12345D686","EngineMake":"MerCruiser","EngineHours":120}', 350000.00, 380000.00, 'Active', 1),
(4, 'Watercraft', '{"VesselType":"Sailboat","Make":"Beneteau","Model":"Oceanis","Length":32,"Year":2020,"HullNumber":"BEN98765F321","EngineMake":"Yanmar","EngineHours":350}', 650000.00, 720000.00, 'Active', 1),
(6, 'Watercraft', '{"VesselType":"Jet Ski","Make":"Yamaha","Model":"FX HO","Length":3.5,"Year":2023,"HullNumber":"YAM45678G901","EngineMake":"Yamaha","EngineHours":45}', 180000.00, 200000.00, 'Active', 1);

-- Aviation Assets (2)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(8, 'Aviation', '{"AircraftType":"Single Engine","Make":"Cessna","Model":"172","Year":2019,"TailNumber":"ZS-ABC","SerialNumber":"172-12345","EngineMake":"Lycoming","EngineHours":1250,"Seats":4}', 1200000.00, 1350000.00, 'Pending', 1),
(10, 'Aviation', '{"AircraftType":"Helicopter","Make":"Robinson","Model":"R44","Year":2021,"TailNumber":"ZS-XYZ","SerialNumber":"R44-67890","EngineMake":"Lycoming","EngineHours":450,"Seats":4}', 1850000.00, 2100000.00, 'Active', 1);

-- Stock/Inventory Assets (3)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(2, 'Stock', '{"Description":"Retail Electronics","SKU":"ELEC-2026","Quantity":1250,"UnitCost":250.00,"StorageLocation":"Warehouse A","Category":"Electronics"}', 312500.00, 350000.00, 'Active', 1),
(5, 'Stock', '{"Description":"Industrial Parts","SKU":"IND-456","Quantity":3500,"UnitCost":85.50,"StorageLocation":"Warehouse B","Category":"Industrial"}', 299250.00, 325000.00, 'Active', 1),
(9, 'Stock', '{"Description":"Pharmaceuticals","SKU":"PHARMA-789","Quantity":800,"UnitCost":450.00,"StorageLocation":"Cold Storage","Category":"Medical","RequiresRefrigeration":true}', 360000.00, 400000.00, 'Pending', 1);

-- Accounts Receivable (3)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(1, 'AccountsReceivable', '{"DebtorName":"Retail Chain SA","TotalOutstanding":450000.00,"NumberOfInvoices":12,"AverageAge":35,"LargestInvoice":85000.00,"PaymentHistory":"Good"}', 425000.00, 450000.00, 'Active', 1),
(4, 'AccountsReceivable', '{"DebtorName":"Construction Supplies CC","TotalOutstanding":780000.00,"NumberOfInvoices":8,"AverageAge":42,"LargestInvoice":210000.00,"PaymentHistory":"Average"}', 740000.00, 780000.00, 'Active', 1),
(7, 'AccountsReceivable', '{"DebtorName":"Tech Solutions Ltd","TotalOutstanding":235000.00,"NumberOfInvoices":5,"AverageAge":28,"LargestInvoice":95000.00,"PaymentHistory":"Good"}', 225000.00, 235000.00, 'Active', 1);

-- Machinery Assets (3)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(6, 'Machinery', '{"Description":"CNC Milling Machine","Make":"Haas","Model":"VF-4","SerialNumber":"HAAS-12345","Year":2022,"Location":"Factory Floor","PowerRequirements":"480V 3Phase","Condition":"Excellent"}', 850000.00, 920000.00, 'Active', 1),
(6, 'Machinery', '{"Description":"Injection Molding Machine","Make":"Arburg","Model":"Allrounder","SerialNumber":"ARB-67890","Year":2020,"Location":"Production Hall","PowerRequirements":"400V 3Phase","Condition":"Good"}', 650000.00, 720000.00, 'Active', 1),
(11, 'Machinery', '{"Description":"Industrial Press Brake","Make":"Amada","Model":"HG-1003","SerialNumber":"AMADA-54321","Year":2021,"Location":"Fabrication Shop","Capacity":"100 ton","Condition":"Excellent"}', 480000.00, 520000.00, 'Active', 1);

-- Plant & Equipment (2)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(2, 'PlantEquipment', '{"Description":"Forklift","Make":"Toyota","Model":"8FGCU25","SerialNumber":"TOY-24680","Year":2023,"Location":"Warehouse","Capacity":"2.5 ton","FuelType":"LPG","Condition":"New"}', 280000.00, 310000.00, 'Active', 1),
(8, 'PlantEquipment', '{"Description":"Generator","Make":"Caterpillar","Model":"C15","SerialNumber":"CAT-13579","Year":2021,"Location":"Backup Power","Capacity":"500 kVA","FuelType":"Diesel","Condition":"Good"}', 420000.00, 460000.00, 'Pending', 1);

-- Business Interruption (2)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(3, 'BusinessInterruption', '{"BusinessName":"Smith Trading","Industry":"Retail","AnnualRevenue":8500000.00,"IndemnityPeriod":12,"GrossProfit":3200000.00,"AdditionalIncreasedCost":500000.00,"WageCosts":1800000.00}', 0.00, 5500000.00, 'Active', 1),
(10, 'BusinessInterruption', '{"BusinessName":"Taylor Manufacturing","Industry":"Manufacturing","AnnualRevenue":15000000.00,"IndemnityPeriod":18,"GrossProfit":5800000.00,"AdditionalIncreasedCost":1200000.00,"WageCosts":3500000.00}', 0.00, 10500000.00, 'Active', 1);

-- Keyman Insurance (2)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(1, 'Keyman', '{"PersonName":"John Smith","Position":"CEO","DateOfBirth":"1975-05-15","Shareholding":35,"AnnualSalary":1800000.00,"InsuredAmount":5000000.00,"Beneficiary":"ABC Corporation"}', 0.00, 5000000.00, 'Active', 1),
(5, 'Keyman', '{"PersonName":"Mary Johnson","Position":"Technical Director","DateOfBirth":"1980-11-20","Shareholding":15,"AnnualSalary":1200000.00,"InsuredAmount":3500000.00,"Beneficiary":"Johnson Trading"}', 0.00, 3500000.00, 'Active', 1);

-- Electronic Equipment (3)
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, Status, CreatedBy)
VALUES 
(2, 'ElectronicEquipment', '{"Description":"Server Room Equipment","Make":"Dell","Model":"PowerEdge R740","SerialNumber":"DELL-98765","Year":2023,"Location":"Data Center","IncludesSoftware":true,"WarrantyExpiry":"2026-12-31"}', 350000.00, 380000.00, 'Active', 1),
(4, 'ElectronicEquipment', '{"Description":"Medical Imaging System","Make":"Siemens","Model":"MAGNETOM Vida","SerialNumber":"SIEM-54321","Year":2022,"Location":"Radiology Dept","IncludesSoftware":true,"WarrantyExpiry":"2027-06-30"}', 2800000.00, 3100000.00, 'Active', 1),
(9, 'ElectronicEquipment', '{"Description":"Security System","Make":"Hikvision","Model":"DS-2CD series","SerialNumber":"HIK-11223","Year":2024,"Location":"Office Building","IncludesSoftware":true,"WarrantyExpiry":"2027-03-01"}', 125000.00, 140000.00, 'Pending', 1);
GO

-- 5. CLAIMS (Sample claims)
INSERT INTO Claims (PolicyId, ClaimNumber, ClaimDate, Description, ClaimAmount, Status, AdjustedAmount, CreatedBy)
VALUES 
(1, 'CLM-2026-001', '2026-02-15', 'Vehicle accident damage - Toyota Hilux', 45000.00, 'Approved', 42500.00, 1),
(1, 'CLM-2026-002', '2026-02-28', 'Theft of electronic equipment', 125000.00, 'Pending', NULL, 1),
(3, 'CLM-2026-003', '2026-03-01', 'Burst pipe water damage', 38000.00, 'Approved', 38000.00, 1),
(4, 'CLM-2026-004', '2026-02-10', 'Marina collision damage', 25000.00, 'Rejected', 0.00, 1),
(6, 'CLM-2026-005', '2026-01-20', 'Machinery breakdown', 75000.00, 'Approved', 72000.00, 1);
GO

-- 6. TRANSACTIONS (Sample billing transactions)
INSERT INTO Transactions (PolicyId, TransactionDate, Description, Amount, TransactionType, Status, Reference)
VALUES 
(1, '2026-01-15', 'Monthly Premium - January', 12500.00, 'Premium', 'Paid', 'INV-2026-001'),
(1, '2026-02-15', 'Monthly Premium - February', 12500.00, 'Premium', 'Paid', 'INV-2026-015'),
(2, '2026-02-20', 'Annual Premium', 45000.00, 'Premium', 'Paid', 'INV-2026-023'),
(3, '2026-03-01', 'Claim Payout - CLM-2026-003', -38000.00, 'Claim', 'Paid', 'PAY-2026-008'),
(6, '2026-01-25', 'Monthly Premium - January', 18750.00, 'Premium', 'Overdue', 'INV-2026-007');
GO

-- 7. AUDIT LOG (Sample entries)
INSERT INTO AuditLog (UserId, Action, TableName, RecordId, Details, Timestamp)
VALUES 
(1, 'CREATE', 'Policy', 1, '{"PolicyNumber":"POL-2026-001"}', DATEADD(HOUR, -48, GETDATE())),
(1, 'UPDATE', 'Policy', 1, '{"Status":"Active"}', DATEADD(HOUR, -24, GETDATE())),
(2, 'CREATE', 'Asset', 1, '{"AssetType":"Vehicle"}', DATEADD(HOUR, -36, GETDATE())),
(1, 'CREATE', 'Claim', 1, '{"ClaimNumber":"CLM-2026-001"}', DATEADD(HOUR, -12, GETDATE()));
GO

PRINT ' Seed data loaded successfully!';
PRINT ' Summary:';
PRINT '  - Partners: 5';
PRINT '  - Users: 7';
PRINT '  - Policies: 15';
PRINT '  - Assets: 30+';
PRINT '  - Claims: 5';
PRINT '  - Transactions: 5';