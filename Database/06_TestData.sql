-- ============================================
-- View Data
-- ============================================

SELECT * FROM Users;

SELECT * FROM Departments;

SELECT * FROM Employees;



-- ============================================
-- View
-- ============================================

SELECT * FROM vw_EmployeeDetails;



-- ============================================
-- Stored Procedures
-- ============================================

EXEC sp_GetEmployeesByDepartment 2;

EXEC sp_SearchEmployees 'Aakash';

EXEC sp_EmployeeCountByDepartment;



-- ============================================
-- Sample Join Query
-- ============================================

SELECT
    E.Name,
    E.Email,
    D.Name AS Department
FROM Employees E
INNER JOIN Departments D
    ON E.DepartmentId = D.Id;