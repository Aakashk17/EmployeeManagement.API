using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Helpers;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.API.Services;

public sealed class DepartmentService(
    IDepartmentRepository departmentRepository,
    IMemoryCache memoryCache,
    IOptions<CacheSettings> cacheOptions) : IDepartmentService
{
    private readonly CacheSettings cacheSettings = cacheOptions.Value;

    public async Task<IReadOnlyCollection<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(CacheKeys.Departments, out IReadOnlyCollection<DepartmentDto>? cachedDepartments) && cachedDepartments is not null)
        {
            return cachedDepartments;
        }

        var departments = await departmentRepository.GetAllAsync(cancellationToken);
        var departmentDtos = departments.Select(ToDto).ToList();
        memoryCache.Set(CacheKeys.Departments, departmentDtos, GetCacheDuration());
        return departmentDtos;
    }

    public async Task<DepartmentDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.DepartmentById(id);
        if (memoryCache.TryGetValue(cacheKey, out DepartmentDto? cachedDepartment) && cachedDepartment is not null)
        {
            return cachedDepartment;
        }

        var department = await departmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Department was not found.");

        var departmentDto = ToDto(department);
        memoryCache.Set(cacheKey, departmentDto, GetCacheDuration());
        return departmentDto;
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(departmentDto);

        if (await departmentRepository.NameExistsAsync(departmentDto.Name.Trim(), excludedDepartmentId: null, cancellationToken))
        {
            throw new InvalidOperationException("Department name is already in use.");
        }

        var department = ToEntity(departmentDto);
        await departmentRepository.AddAsync(department, cancellationToken);
        InvalidateCache(department.Id);
        return ToDto(department);
    }

    public async Task<DepartmentDto> UpdateAsync(int id, DepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(departmentDto);

        var department = await departmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Department was not found.");

        if (await departmentRepository.NameExistsAsync(departmentDto.Name.Trim(), id, cancellationToken))
        {
            throw new InvalidOperationException("Department name is already in use.");
        }

        department.Name = departmentDto.Name.Trim();
        department.Description = string.IsNullOrWhiteSpace(departmentDto.Description) ? null : departmentDto.Description.Trim();

        await departmentRepository.UpdateAsync(department, cancellationToken);
        InvalidateCache(id);
        return ToDto(department);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Department was not found.");

        if (await departmentRepository.HasEmployeesAsync(id, cancellationToken))
        {
            throw new InvalidOperationException("Department cannot be deleted while employees are assigned to it.");
        }

        await departmentRepository.DeleteAsync(department, cancellationToken);
        InvalidateCache(id);
    }

    private void InvalidateCache(int departmentId)
    {
        memoryCache.Remove(CacheKeys.Departments);
        memoryCache.Remove(CacheKeys.DepartmentById(departmentId));
    }

    private TimeSpan GetCacheDuration()
    {
        return TimeSpan.FromMinutes(Math.Max(1, cacheSettings.DurationMinutes));
    }

    private static DepartmentDto ToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            CreatedDate = department.CreatedDate
        };
    }

    private static Department ToEntity(DepartmentDto departmentDto)
    {
        return new Department
        {
            Name = departmentDto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(departmentDto.Description) ? null : departmentDto.Description.Trim(),
            CreatedDate = DateTime.Now
        };
    }
}
