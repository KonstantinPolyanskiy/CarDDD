using System.Reflection;
using CarDDD.ApplicationServices.Dispatchers;
using CarDDD.ApplicationServices.Repositories;
using CarDDD.ApplicationServices.Services;
using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarDDD.ApplicationServices.Extensions;

public static class DomainHandlersDependencyInjection
{
    public static IServiceCollection AddDomainHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<IDomainEventDispatcher, SimpleDomainEventDispatcher>();
        
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IDomainEventHandler<>))
            )
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );

        return services;
    }
}

public static class ApplicationServicesDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<ICarRepositoryReader, CarRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        
        services.AddScoped<ICarDomainService, DomainCarService>();
        services.AddScoped<ICartDomainService, DomainCartService>();
        
        services
            .AddScoped<ICartMutableService, CartMutableService>()
            .AddScoped<ICarQueryService, CarQueryService>()
            .AddScoped<ICarMutableService, CarMutableService>();
        
        return services;
    }
}
