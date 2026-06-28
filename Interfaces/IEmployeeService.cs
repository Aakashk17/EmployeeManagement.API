using EmployeeManagement.API.DTOs;

namespace EmployeeManagement.API.Interfaces;

public interface IEmployeeService
{
    Task<IReadOnlyCollection<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<EmployeeDto> CreateAsync(EmployeeDto employeeDto, CancellationToken cancellationToken);

    Task<EmployeeDto> UpdateAsync(int id, EmployeeDto employeeDto, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
