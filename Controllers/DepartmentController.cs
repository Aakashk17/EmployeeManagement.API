using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.User}")]
[Route("api/[controller]")]
public sealed class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<DepartmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<DepartmentDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var departments = await departmentService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<DepartmentDto>>.Ok(departments));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var department = await departmentService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DepartmentDto>.Ok(department));
    }

    [Authorize(Roles = RoleConstants.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create(DepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var department = await departmentService.CreateAsync(departmentDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, ApiResponse<DepartmentDto>.Ok(department, "Department created successfully."));
    }

    [Authorize(Roles = RoleConstants.Admin)]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Update(int id, DepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var department = await departmentService.UpdateAsync(id, departmentDto, cancellationToken);
        return Ok(ApiResponse<DepartmentDto>.Ok(department, "Department updated successfully."));
    }

    [Authorize(Roles = RoleConstants.Admin)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await departmentService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "Department deleted successfully."));
    }
}
