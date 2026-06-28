-- ============================================
-- Get Employees By Department
-- ============================================

CREATE PROCEDURE sp_GetEmployeesByDepartment
    @DepartmentId INT
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
        E.Id,
        E.Name,
        E.Email,
        E.Phone,
        E.Salary,
        D.Name AS DepartmentName
    FROM Employees E
    INNER JOIN Departments D
        ON E.DepartmentId = D.Id
    WHERE E.DepartmentId = @DepartmentId
    AND E.IsActive = 1;

END;
GO



-- ============================================
-- Search Employee
-- ============================================

CREATE PROCEDURE sp_SearchEmployees
    @SearchText NVARCHAR(100)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
        E.Id,
        E.Name,
        E.Email,
        E.Phone,
        E.Salary,
        D.Name AS DepartmentName
    FROM Employees E
    INNER JOIN Departments D
        ON E.DepartmentId = D.Id
    WHERE E.Name LIKE '%' + @SearchText + '%'
       OR E.Email LIKE '%' + @SearchText + '%';

END;
GO



-- ============================================
-- Employee Count By Department
-- ============================================

CREATE PROCEDURE sp_EmployeeCountByDepartment
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
        D.Name,
        COUNT(E.Id) AS EmployeeCount
    FROM Departments D
    LEFT JOIN Employees E
        ON D.Id = E.DepartmentId
    GROUP BY D.Name;

END;
GO