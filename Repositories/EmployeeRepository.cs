using EmployeeManagement.API.Data;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Repositories;

public sealed class EmployeeRepository(ApplicationDbContext dbContext) : IEmployeeRepository
{
    public async Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .OrderBy(employee => employee.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Employees
            .Include(employee => employee.Department)
            .FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken);
    }

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee, CancellationToken cancellationToken)
    {
        dbContext.Employees.Update(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task DeleteAsync(Employee employee, CancellationToken cancellationToken)
    {
        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludedEmployeeId, CancellationToken cancellationToken)
    {
        return await dbContext.Employees.AnyAsync(
            employee => employee.Email == email && (!excludedEmployeeId.HasValue || employee.Id != excludedEmployeeId.Value),
            cancellationToken);
    }
}
