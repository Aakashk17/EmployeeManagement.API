using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models;

public sealed class Department
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
