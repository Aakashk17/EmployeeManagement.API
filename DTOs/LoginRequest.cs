using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs;

public sealed class LoginRequest
{
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    public string Password { get; set; } = string.Empty;
}
