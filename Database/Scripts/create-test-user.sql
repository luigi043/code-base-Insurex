-- Run this in SSMS against your InsureX database
USE InsureX;

-- Check if user exists
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@insurex.com')
BEGIN
    -- Insert test user (password: "password" - you'll need to hash it properly)
    -- Note: This is a placeholder. In production, use proper password hashing
    INSERT INTO Users (Email, PasswordHash, Role, IsActive, CreatedAt)
    VALUES ('admin@insurex.com', 'AQAAAAIAAYagAAAAEK3wKQ8Qx9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q9Q', 'Admin', 1, GETDATE());
    
    PRINT '✅ Test user created';
END
ELSE
BEGIN
    PRINT '⚠️ Test user already exists';
END

-- View all users
SELECT * FROM Users;