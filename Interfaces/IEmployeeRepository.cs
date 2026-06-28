using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Interfaces;

public interface IEmployeeRepository
{
    Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken);

    Task<Employee> UpdateAsync(Employee employee, CancellationToken cancellationToken);

    Task DeleteAsync(Employee employee, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, int? excludedEmployeeId, CancellationToken cancellationToken);
}
