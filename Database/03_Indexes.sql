-- ============================================
-- Employees Indexes
-- ============================================

CREATE INDEX IX_Employees_Email
ON Employees (Email);

CREATE INDEX IX_Employees_DepartmentId
ON Employees (DepartmentId);

CREATE INDEX IX_Employees_IsActive
ON Employees (IsActive);



-- ============================================
-- Users Indexes
-- ============================================

CREATE INDEX IX_Users_Email
ON Users (Email);

CREATE INDEX IX_Users_Username
ON Users (Username);