using Azure.Core;
using Azure.Identity;
using EmployeeManagement.API.Settings;

namespace EmployeeManagement.API.Helpers;

public static class AzureCredentialFactory
{
    public static TokenCredential CreateDefaultCredential(KeyVaultSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.TenantId))
        {
            return new DefaultAzureCredential();
        }

        return new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            VisualStudioTenantId = settings.TenantId,
            SharedTokenCacheTenantId = settings.TenantId
        });
    }
}
