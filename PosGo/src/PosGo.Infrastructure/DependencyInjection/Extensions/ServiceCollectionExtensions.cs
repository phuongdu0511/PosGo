using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PosGo.Application.Abstractions;
using PosGo.Infrastructure.Authentication;
using PosGo.Infrastructure.Caching;
using PosGo.Infrastructure.Services;

namespace PosGo.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServicesInfrastructure(this IServiceCollection services)
        => services.AddTransient<IJwtTokenService, JwtTokenService>()
            .AddTransient<ICacheService, CacheService>()
            .AddScoped<IPermissionService, PermissionService>();

    public static void AddRedisInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connectionString;
        });
    }
}
