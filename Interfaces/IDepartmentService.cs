using EmployeeManagement.API.DTOs;

namespace EmployeeManagement.API.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyCollection<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<DepartmentDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<DepartmentDto> CreateAsync(DepartmentDto departmentDto, CancellationToken cancellationToken);

    Task<DepartmentDto> UpdateAsync(int id, DepartmentDto departmentDto, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
