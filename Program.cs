using EmployeeManagement.API.Extensions;
using EmployeeManagement.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEnabled = builder.Configuration.GetValue<bool>("KeyVaultSettings:Enabled");

if (keyVaultEnabled)
{
    await builder.Configuration.AddKeyVaultSecretsAsync();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationSettings(builder.Configuration);
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddMemoryCache();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSwaggerDocumentation();

builder.Host.UseApplicationSerilog();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting EmployeeManagement.API");
    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "EmployeeManagement.API terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
