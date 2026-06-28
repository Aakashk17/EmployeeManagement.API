using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Helpers;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.API.Services;

public sealed class AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings jwtSettings = jwtOptions.Value;

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userRepository.GetByUserNameAsync(request.UserName.Trim(), cancellationToken);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes);

        return new LoginResponse
        {
            Token = CreateToken(user, expiresAtUtc),
            ExpiresAtUtc = expiresAtUtc,
            UserName = user.UserName,
            Role = user.Role
        };
    }

    public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userName = request.UserName.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (await userRepository.UserNameExistsAsync(userName, cancellationToken))
        {
            throw new InvalidOperationException("Username is already registered.");
        }

        if (await userRepository.EmailExistsAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = RoleConstants.User,
            CreatedDate = DateTime.Now
        };

        await userRepository.AddAsync(user, cancellationToken);
    }

    private string CreateToken(User user, DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
        {
            throw new InvalidOperationException("JWT secret key is not configured.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
