using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Abstractions;
using PosGo.Persistence.DependencyInjection.Options;
using PosGo.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PosGo.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using PosGo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using PosGo.Persistence.ErrorDescriber;

namespace PosGo.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSqlServerPersistence(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>((provider, builder) =>
        {
            // Interceptor
            var auditableInterceptor = provider.GetService<UpdateAuditableEntitiesInterceptor>();

            var configuration = provider.GetRequiredService<IConfiguration>();
            var options = provider.GetRequiredService<IOptionsMonitor<SqlServerRetryOptions>>();

            builder
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(true)
            .UseLazyLoadingProxies(true) // => If UseLazyLoadingProxies, all of the navigation fields should be VIRTUAL
            .UseSqlServer(
                connectionString: configuration.GetConnectionString("ConnectionStrings"),
                sqlServerOptionsAction: optionsBuilder
                        => optionsBuilder.ExecutionStrategy(
                                dependencies => new SqlServerRetryingExecutionStrategy(
                                    dependencies: dependencies,
                                    maxRetryCount: options.CurrentValue.MaxRetryCount,
                                    maxRetryDelay: options.CurrentValue.MaxRetryDelay,
                                    errorNumbersToAdd: options.CurrentValue.ErrorNumbersToAdd))
                            .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name))
            .AddInterceptors(auditableInterceptor);
        });

        services.AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            /*
             *   Property             |                    Description
             * RequireDigit           |  Requires a number between 0-9 in the password.
             * RequiredLength         |  The minimum length of the password.
             * RequireLowercase       |  Requires a lowercase character in the password.
             * RequireUppercase       |  Requires an uppercase character in the password.
             * RequireNonAlphanumeric |  Requires a non-alphanumeric character in the password.
             * RequiredUniqueChars    |  (Only applies to ASP.NET Core 2.0 or later.) Requires the number of distinct characters in the password.
             */

            // Default Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            //options.Password.RequireDigit = passwordValidatorOptions.CurrentValue.RequireDigitLength >= 1 ? true : false;
            //options.Password.RequireLowercase = passwordValidatorOptions.CurrentValue.RequireLowercaseLength >= 1 ? true : false;
            //options.Password.RequireNonAlphanumeric = passwordValidatorOptions.CurrentValue.RequireNonLetterOrDigitLength >= 1 ? true : false;
            //options.Password.RequireUppercase = passwordValidatorOptions.CurrentValue.RequireUppercaseLength >= 1 ? true : false;
            //options.Password.RequiredLength = passwordValidatorOptions.CurrentValue.RequiredMinLength;
            //options.Password.RequiredUniqueChars = 1;
        });
    }

    public static void AddInterceptorPersistence(this IServiceCollection services)
    {
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<UpdateAuditableEntitiesInterceptor>();
    }

    public static void AddRepositoryPersistence(this IServiceCollection services)
    {
        services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
        services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));

        services.AddTransient(typeof(IUnitOfWorkDbContext<>), typeof(EFUnitOfWorkDbContext<>));
        services.AddTransient(typeof(IRepositoryBaseDbContext<,,>), typeof(RepositoryBaseDbContext<,,>));

    }

    public static OptionsBuilder<SqlServerRetryOptions> ConfigureSqlServerRetryOptionsPersistence(this IServiceCollection services, IConfigurationSection section)
        => services
            .AddOptions<SqlServerRetryOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}
