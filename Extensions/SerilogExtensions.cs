using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace EmployeeManagement.API.Extensions;

public static class SerilogExtensions
{
    public static ConfigureHostBuilder UseApplicationSerilog(this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    restrictedToMinimumLevel: LogEventLevel.Information);

            var applicationInsightsConnectionString = context.Configuration["ApplicationInsights:ConnectionString"];
            if (!string.IsNullOrWhiteSpace(applicationInsightsConnectionString))
            {
                var telemetryConfiguration = services.GetService<TelemetryConfiguration>();
                if (telemetryConfiguration is not null)
                {
                    loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
                }
            }
        });

        return hostBuilder;
    }
}
