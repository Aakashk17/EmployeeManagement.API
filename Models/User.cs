using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models;

public sealed class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = RoleConstants.User;

    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
