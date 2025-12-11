using Carter;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using PosGo.API.DependencyInjection.Extensions;
using PosGo.API.Middleware;
using PosGo.Application.DependencyInjection.Extensions;
using PosGo.Infrastructure.DependencyInjection.Extensions;
using PosGo.Persistence.DependencyInjection.Extensions;
using PosGo.Persistence.DependencyInjection.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add configuration

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

builder
    .Services
    .AddControllers()
    .AddApplicationPart(PosGo.Presentation.AssemblyReference.Assembly);

builder.Services
        .AddSwaggerGenNewtonsoftSupport()
        .AddFluentValidationRulesToSwagger()
        .AddEndpointsApiExplorer()
        .AddSwaggerAPI();

builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddJwtAuthenticationAPI(builder.Configuration);

builder.Services.AddServicesInfrastructure();
builder.Services.AddRedisInfrastructure(builder.Configuration);

builder.Services.AddMediatRApplication();
builder.Services.AddAutoMapperApplication();

// Configure Options and SQL
builder.Services.AddInterceptorPersistence();
builder.Services.ConfigureSqlServerRetryOptionsPersistence(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddSqlServerPersistence();
builder.Services.AddRepositoryPersistence();

builder.Services.AddCarter();

// Add Middleware
builder.Services.AddOpenApi();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

// Add API Endpoint with carter module
app.MapCarter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
    app.UseSwaggerAPI();

try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}
