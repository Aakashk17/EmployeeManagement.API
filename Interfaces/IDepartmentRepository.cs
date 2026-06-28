using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken cancellationToken);

    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Department> AddAsync(Department department, CancellationToken cancellationToken);

    Task<Department> UpdateAsync(Department department, CancellationToken cancellationToken);

    Task DeleteAsync(Department department, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, int? excludedDepartmentId, CancellationToken cancellationToken);

    Task<bool> HasEmployeesAsync(int id, CancellationToken cancellationToken);
}
