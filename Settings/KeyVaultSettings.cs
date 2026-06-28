namespace EmployeeManagement.API.Settings;

public sealed class KeyVaultSettings
{
    public bool Enabled { get; set; }

    public string VaultUri { get; set; } = string.Empty;

    public string ConnectionStringSecretName { get; set; } = "SqlConnectionString";

    public string JwtSecretKeySecretName { get; set; } = "JwtSecretKey";

    public string TenantId { get; set; } = string.Empty;
}
