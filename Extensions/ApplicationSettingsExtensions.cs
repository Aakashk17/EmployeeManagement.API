using EmployeeManagement.API.Settings;

namespace EmployeeManagement.API.Extensions;

public static class ApplicationSettingsExtensions
{
    public static IServiceCollection AddApplicationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<CacheSettings>(configuration.GetSection(nameof(CacheSettings)));
        services.Configure<KeyVaultSettings>(configuration.GetSection(nameof(KeyVaultSettings)));
        return services;
    }
}
