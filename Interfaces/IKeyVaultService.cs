namespace EmployeeManagement.API.Interfaces;

public interface IKeyVaultService
{
    Task<string?> GetSecretAsync(string secretName, CancellationToken cancellationToken);

    Task<string?> GetDefaultConnectionStringAsync(CancellationToken cancellationToken);

    Task<string?> GetJwtSecretKeyAsync(CancellationToken cancellationToken);
}
