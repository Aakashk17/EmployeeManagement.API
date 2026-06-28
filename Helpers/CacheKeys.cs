namespace EmployeeManagement.API.Helpers;

public static class CacheKeys
{
    public const string Employees = "employees:all";

    public const string Departments = "departments:all";

    public static string EmployeeById(int id)
    {
        return $"employees:{id}";
    }

    public static string DepartmentById(int id)
    {
        return $"departments:{id}";
    }
}
