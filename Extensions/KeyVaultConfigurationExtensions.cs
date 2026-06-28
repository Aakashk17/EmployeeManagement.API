using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using EmployeeManagement.API.Helpers;
using EmployeeManagement.API.Settings;

namespace EmployeeManagement.API.Extensions;

public static class KeyVaultConfigurationExtensions
{
    public static async Task AddKeyVaultSecretsAsync(this ConfigurationManager configuration)
    {
        var settings = configuration.GetSection(nameof(KeyVaultSettings)).Get<KeyVaultSettings>() ?? new KeyVaultSettings();
        if (!settings.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(settings.VaultUri))
        {
            throw new InvalidOperationException("KeyVaultSettings:VaultUri is required when Key Vault is enabled.");
        }

        try
        {
            var secretClient = new SecretClient(new Uri(settings.VaultUri), AzureCredentialFactory.CreateDefaultCredential(settings));
            var secretValues = new Dictionary<string, string?>();

            await AddSecretAsync(secretClient, settings.ConnectionStringSecretName, "ConnectionStrings:DefaultConnection", secretValues);
            await AddSecretAsync(secretClient, settings.JwtSecretKeySecretName, "JwtSettings:SecretKey", secretValues);

            if (secretValues.Count > 0)
            {
                configuration.AddInMemoryCollection(secretValues);
            }
        }
        catch (CredentialUnavailableException exception)
        {
            throw new InvalidOperationException(GetCredentialErrorMessage(settings), exception);
        }
        catch (AuthenticationFailedException exception)
        {
            throw new InvalidOperationException(GetCredentialErrorMessage(settings), exception);
        }
        catch (RequestFailedException exception) when (exception.Status is 401 or 403)
        {
            throw new InvalidOperationException("Azure Key Vault access was denied. Grant this signed-in identity Get/List secret permissions on the vault.", exception);
        }
    }

    private static async Task AddSecretAsync(
        SecretClient secretClient,
        string secretName,
        string configurationKey,
        IDictionary<string, string?> secretValues)
    {
        if (string.IsNullOrWhiteSpace(secretName))
        {
            return;
        }

        var response = await secretClient.GetSecretAsync(secretName);
        secretValues[configurationKey] = response.Value.Value;
    }

    private static string GetCredentialErrorMessage(KeyVaultSettings settings)
    {
        var tenantMessage = string.IsNullOrWhiteSpace(settings.TenantId)
            ? " If your account belongs to a specific Azure tenant, set KeyVaultSettings:TenantId."
            : string.Empty;

        return "Azure Key Vault authentication failed. Re-authenticate Visual Studio under Tools > Options > Azure Services Authentication, or sign in with Azure CLI/Azure PowerShell, or run the app with a managed identity/service principal." + tenantMessage;
    }
}
