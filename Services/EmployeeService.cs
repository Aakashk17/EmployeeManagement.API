using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Helpers;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.API.Services;

public sealed class EmployeeService(
    IEmployeeRepository employeeRepository,
    IDepartmentRepository departmentRepository,
    IMemoryCache memoryCache,
    IOptions<CacheSettings> cacheOptions) : IEmployeeService
{
    private readonly CacheSettings cacheSettings = cacheOptions.Value;

    public async Task<IReadOnlyCollection<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(CacheKeys.Employees, out IReadOnlyCollection<EmployeeDto>? cachedEmployees) && cachedEmployees is not null)
        {
            return cachedEmployees;
        }

        var employees = await employeeRepository.GetAllAsync(cancellationToken);
        var employeeDtos = employees.Select(ToDto).ToList();
        memoryCache.Set(CacheKeys.Employees, employeeDtos, GetCacheDuration());
        return employeeDtos;
    }

    public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.EmployeeById(id);
        if (memoryCache.TryGetValue(cacheKey, out EmployeeDto? cachedEmployee) && cachedEmployee is not null)
        {
            return cachedEmployee;
        }

        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Employee was not found.");

        var employeeDto = ToDto(employee);
        memoryCache.Set(cacheKey, employeeDto, GetCacheDuration());
        return employeeDto;
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeDto employeeDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(employeeDto);
        await ValidateEmployeeAsync(employeeDto, excludedEmployeeId: null, cancellationToken);

        var employee = ToEntity(employeeDto);
        await employeeRepository.AddAsync(employee, cancellationToken);
        InvalidateCache(employee.Id);

        var savedEmployee = await employeeRepository.GetByIdAsync(employee.Id, cancellationToken)
            ?? throw new InvalidOperationException("Employee was saved but could not be loaded.");

        return ToDto(savedEmployee);
    }

    public async Task<EmployeeDto> UpdateAsync(int id, EmployeeDto employeeDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(employeeDto);

        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Employee was not found.");

        await ValidateEmployeeAsync(employeeDto, id, cancellationToken);

        employee.Name = employeeDto.Name.Trim();
        employee.Email = employeeDto.Email.Trim().ToLowerInvariant();
        employee.Phone = string.IsNullOrWhiteSpace(employeeDto.Phone) ? null : employeeDto.Phone.Trim();
        employee.Salary = employeeDto.Salary;
        employee.DepartmentId = employeeDto.DepartmentId;
        employee.IsActive = employeeDto.IsActive;

        await employeeRepository.UpdateAsync(employee, cancellationToken);
        InvalidateCache(id);

        var updatedEmployee = await employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Employee was updated but could not be loaded.");

        return ToDto(updatedEmployee);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Employee was not found.");

        await employeeRepository.DeleteAsync(employee, cancellationToken);
        InvalidateCache(id);
    }

    private async Task ValidateEmployeeAsync(EmployeeDto employeeDto, int? excludedEmployeeId, CancellationToken cancellationToken)
    {
        if (!await departmentRepository.ExistsAsync(employeeDto.DepartmentId, cancellationToken))
        {
            throw new InvalidOperationException("Department does not exist.");
        }

        if (await employeeRepository.EmailExistsAsync(employeeDto.Email.Trim().ToLowerInvariant(), excludedEmployeeId, cancellationToken))
        {
            throw new InvalidOperationException("Employee email is already in use.");
        }
    }

    private void InvalidateCache(int employeeId)
    {
        memoryCache.Remove(CacheKeys.Employees);
        memoryCache.Remove(CacheKeys.EmployeeById(employeeId));
    }

    private TimeSpan GetCacheDuration()
    {
        return TimeSpan.FromMinutes(Math.Max(1, cacheSettings.DurationMinutes));
    }

    private static EmployeeDto ToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Phone = employee.Phone,
            Salary = employee.Salary,
            DepartmentId = employee.DepartmentId,
            IsActive = employee.IsActive,
            CreatedDate = employee.CreatedDate,
            DepartmentName = employee.Department?.Name
        };
    }

    private static Employee ToEntity(EmployeeDto employeeDto)
    {
        return new Employee
        {
            Name = employeeDto.Name.Trim(),
            Email = employeeDto.Email.Trim().ToLowerInvariant(),
            Phone = string.IsNullOrWhiteSpace(employeeDto.Phone) ? null : employeeDto.Phone.Trim(),
            Salary = employeeDto.Salary,
            DepartmentId = employeeDto.DepartmentId,
            IsActive = employeeDto.IsActive,
            CreatedDate = DateTime.Now
        };
    }
}
