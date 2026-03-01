-- Enhanced Seed Data for InsureX
USE InsureX;
GO

-- Clear existing data (optional - comment out if you want to keep existing)
-- DELETE FROM Claims;
-- DELETE FROM Assets;
-- DELETE FROM Policies;
-- DELETE FROM Users;
-- GO

-- Insert more users
INSERT INTO Users (Email, PasswordHash, Role, CreatedAt)
VALUES 
('admin@insurex.com', 'AQAAAAIAAYagAAAAE...', 'Admin', GETDATE()),
('manager@insurex.com', 'AQAAAAIAAYagAAAAE...', 'Manager', GETDATE()),
('user1@example.com', 'AQAAAAIAAYagAAAAE...', 'User', GETDATE()),
('user2@example.com', 'AQAAAAIAAYagAAAAE...', 'User', GETDATE());
GO

-- Insert more policies
INSERT INTO Policies (PolicyNumber, CustomerName, StartDate, EndDate, Status, TotalInsuredValue, CreatedAt)
VALUES 
('POL-2026-001', 'ABC Corporation', '2026-01-01', '2027-01-01', 'Active', 5000000, GETDATE()),
('POL-2026-002', 'XYZ Enterprises', '2026-02-15', '2027-02-15', 'Active', 2500000, GETDATE()),
('POL-2026-003', 'Smith Family', '2026-03-01', '2027-03-01', 'Pending', 750000, GETDATE()),
('POL-2026-004', 'Johnson Plumbing', '2026-01-15', '2027-01-15', 'Active', 1200000, GETDATE()),
('POL-2026-005', 'Beachfront Properties', '2026-02-01', '2027-02-01', 'Suspended', 3500000, GETDATE()),
('POL-2026-006', 'Tech Innovations Inc', '2026-03-10', '2027-03-10', 'Active', 8750000, GETDATE());
GO

-- Insert assets
INSERT INTO Assets (PolicyId, AssetType, AssetData, FinanceValue, InsuredValue, CreatedAt)
VALUES 
(1, 'Vehicle', '{"Make":"Toyota","Model":"Camry","Year":2023,"VIN":"4T1BF1FK8PU123456"}', 25000, 28000, GETDATE()),
(1, 'Vehicle', '{"Make":"Honda","Model":"CR-V","Year":2024,"VIN":"5J6RW1H89PA123456"}', 32000, 35000, GETDATE()),
(2, 'Property', '{"Address":"123 Main St","City":"Springfield","Type":"Commercial","Size":2500}', 450000, 475000, GETDATE()),
(3, 'Vehicle', '{"Make":"Ford","Model":"F-150","Year":2022,"VIN":"1FTFW1E85NFA12345"}', 38000, 40000, GETDATE()),
(4, 'Machinery', '{"Type":"Excavator","Model":"CAT 320","Year":2021,"Serial":"CAT3202021-001"}', 95000, 102000, GETDATE()),
(5, 'Property', '{"Address":"456 Beach Rd","City":"Miami","Type":"Residential","Size":1800}', 320000, 340000, GETDATE()),
(6, 'ElectronicEquipment', '{"Type":"Server","Model":"Dell PowerEdge","Serial":"DELL-R740-001"}', 15000, 18000, GETDATE()),
(6, 'ElectronicEquipment', '{"Type":"Workstations","Quantity":25,"Model":"Dell Optiplex"}', 37500, 40000, GETDATE());
GO

-- Insert claims
INSERT INTO Claims (PolicyId, ClaimNumber, ClaimDate, Amount, Status, Description, CreatedAt)
VALUES 
(1, 'CLM-2026-001', '2026-02-15', 3500, 'Approved', 'Windshield damage', GETDATE()),
(3, 'CLM-2026-002', '2026-02-28', 12500, 'Pending', 'Engine failure', GETDATE()),
(5, 'CLM-2026-003', '2026-01-20', 8750, 'Paid', 'Storm damage to roof', GETDATE());
GO

-- Display counts
SELECT 'Users' as TableName, COUNT(*) as Count FROM Users
UNION ALL
SELECT 'Policies', COUNT(*) FROM Policies
UNION ALL
SELECT 'Assets', COUNT(*) FROM Assets
UNION ALL
SELECT 'Claims', COUNT(*) FROM Claims;
GO