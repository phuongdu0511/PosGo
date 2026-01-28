using Amazon.S3;
using Amazon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PosGo.Application.Abstractions;
using PosGo.Infrastructure.Authentication;
using PosGo.Infrastructure.Caching;
using PosGo.Infrastructure.DependencyInjection.Options;
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

    // S3 Service
    public static void AddAmazonS3Infrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3Option>(configuration.GetSection(nameof(S3Option)));
        var s3Options = configuration.GetSection(nameof(S3Option)).Get<S3Option>();
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Options.Region)
            };
            return new AmazonS3Client(config);
        });

        services.AddScoped<IS3Service, S3Service>();
    }
}
