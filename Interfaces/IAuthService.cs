using EmployeeManagement.API.DTOs;

namespace EmployeeManagement.API.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
}
