namespace EmployeeManagement.API.Models;

public static class RoleConstants
{
    public const string Admin = "Admin";
    public const string User = "User";

    public static bool IsValid(string role)
    {
        return string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase)
            || string.Equals(role, User, StringComparison.OrdinalIgnoreCase);
    }

    public static string Normalize(string? role)
    {
        if (string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase))
        {
            return Admin;
        }

        return User;
    }
}
