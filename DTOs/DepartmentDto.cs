using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs;

public sealed class DepartmentDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }
}
