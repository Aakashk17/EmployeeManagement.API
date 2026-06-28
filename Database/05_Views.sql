-- ============================================
-- Employee Details View
-- ============================================

CREATE VIEW vw_EmployeeDetails
AS

SELECT
    E.Id,
    E.Name,
    E.Email,
    E.Phone,
    E.Salary,
    E.IsActive,
    D.Name AS DepartmentName,
    E.CreatedDate
FROM Employees E
INNER JOIN Departments D
    ON E.DepartmentId = D.Id;
GO