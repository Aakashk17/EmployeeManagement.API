using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using EmployeeManagement.API.Helpers;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Settings;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.API.Services;

public sealed class KeyVaultService : IKeyVaultService
{
    private readonly KeyVaultSettings settings;
    private readonly SecretClient? secretClient;

    public KeyVaultService(IOptions<KeyVaultSettings> options)
    {
        settings = options.Value;
        secretClient = CreateSecretClient(settings);
    }

    public async Task<string?> GetSecretAsync(string secretName, CancellationToken cancellationToken)
    {
        if (secretClient is null || string.IsNullOrWhiteSpace(secretName))
        {
            return null;
        }

        try
        {
            var response = await secretClient.GetSecretAsync(secretName, cancellationToken: cancellationToken);
            return response.Value.Value;
        }
        catch (CredentialUnavailableException exception)
        {
            throw new InvalidOperationException("Azure Key Vault authentication failed !. Re-authenticate your Azure developer identity or configure managed identity/service principal credentials.", exception);
        }
        catch (AuthenticationFailedException exception)
        {
            throw new InvalidOperationException("Azure Key Vault authentication failed. Re-authenticate your Azure developer identity or configure managed identity/service principal credentials.", exception);
        }
        catch (RequestFailedException exception) when (exception.Status is 401 or 403)
        {
            throw new InvalidOperationException("Azure Key Vault access was denied. Grant this identity permission to read secrets.", exception);
        }
    }

    public Task<string?> GetDefaultConnectionStringAsync(CancellationToken cancellationToken)
    {
        return GetSecretAsync(settings.ConnectionStringSecretName, cancellationToken);
    }

    public Task<string?> GetJwtSecretKeyAsync(CancellationToken cancellationToken)
    {
        return GetSecretAsync(settings.JwtSecretKeySecretName, cancellationToken);
    }

    private static SecretClient? CreateSecretClient(KeyVaultSettings settings)
    {
        if (!settings.Enabled || string.IsNullOrWhiteSpace(settings.VaultUri))
        {
            return null;
        }

        return new SecretClient(new Uri(settings.VaultUri), AzureCredentialFactory.CreateDefaultCredential(settings));
    }
}
