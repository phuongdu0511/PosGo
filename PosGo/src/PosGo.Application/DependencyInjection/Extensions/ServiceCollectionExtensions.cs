using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PosGo.Application.Behaviors;
using PosGo.Application.Mapper;
using PosGo.Domain.Entities;

namespace PosGo.Application.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRApplication(this IServiceCollection services)
        => services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly))
        //.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationDefaultBehavior<,>))
        .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
        .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformancePipelineBehavior<,>))
        .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionPipelineBehavior<,>))
        //.AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingPipelineBehavior<,>))
        .AddValidatorsFromAssembly(Contract.AssemblyReference.Assembly, includeInternalTypes: true);

    public static IServiceCollection AddAutoMapperApplication(this IServiceCollection services)
        => services.AddAutoMapper(typeof(ServiceProfile));

    public static IServiceCollection AddPasswordHaserApplication(this IServiceCollection services)
        => services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

}
