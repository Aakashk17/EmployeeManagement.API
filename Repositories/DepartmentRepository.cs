using EmployeeManagement.API.Data;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Repositories;

public sealed class DepartmentRepository(ApplicationDbContext dbContext) : IDepartmentRepository
{
    public async Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Departments
            .AsNoTracking()
            .OrderBy(department => department.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Departments.FirstOrDefaultAsync(department => department.Id == id, cancellationToken);
    }

    public async Task<Department> AddAsync(Department department, CancellationToken cancellationToken)
    {
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync(cancellationToken);
        return department;
    }

    public async Task<Department> UpdateAsync(Department department, CancellationToken cancellationToken)
    {
        dbContext.Departments.Update(department);
        await dbContext.SaveChangesAsync(cancellationToken);
        return department;
    }

    public async Task DeleteAsync(Department department, CancellationToken cancellationToken)
    {
        dbContext.Departments.Remove(department);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Departments.AnyAsync(department => department.Id == id, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludedDepartmentId, CancellationToken cancellationToken)
    {
        return await dbContext.Departments.AnyAsync(
            department => department.Name == name && (!excludedDepartmentId.HasValue || department.Id != excludedDepartmentId.Value),
            cancellationToken);
    }

    public async Task<bool> HasEmployeesAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Employees.AnyAsync(employee => employee.DepartmentId == id, cancellationToken);
    }
}
