-- Fixed Seed Data for InsureX (matches actual schema)
USE InsureX;
GO

-- Check if Users table has IsActive column, add if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'IsActive')
BEGIN
    ALTER TABLE Users ADD IsActive BIT NOT NULL DEFAULT 1;
    PRINT 'Added IsActive column to Users';
END
GO

-- Insert more users (using correct columns)
INSERT INTO Users (Email, PasswordHash, Role, IsActive, CreatedAt)
SELECT * FROM (VALUES 
    ('admin@insurex.com', 'AQAAAAIAAYagAAAAE...', 'Admin', 1, GETDATE()),
    ('manager@insurex.com', 'AQAAAAIAAYagAAAAE...', 'Manager', 1, GETDATE()),
    ('user1@example.com', 'AQAAAAIAAYagAAAAE...', 'User', 1, GETDATE()),
    ('user2@example.com', 'AQAAAAIAAYagAAAAE...', 'User', 1, GETDATE())
) AS src(Email, PasswordHash, Role, IsActive, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = src.Email);
GO

-- Check Policies table columns and insert appropriate data
PRINT 'Current Policies count: ' + CAST((SELECT COUNT(*) FROM Policies) AS VARCHAR);
GO

-- Check Assets table
PRINT 'Current Assets count: ' + CAST((SELECT COUNT(*) FROM Assets) AS VARCHAR);
GO

-- Display final counts
SELECT 'Users' as TableName, COUNT(*) as Count FROM Users
UNION ALL
SELECT 'Policies', COUNT(*) FROM Policies
UNION ALL
SELECT 'Assets', COUNT(*) FROM Assets
UNION ALL
SELECT 'Claims', COUNT(*) FROM Claims;
GO
