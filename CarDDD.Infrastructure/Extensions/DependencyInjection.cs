using CarDDD.ApplicationServices.Publishers;
using CarDDD.ApplicationServices.Storages;
using CarDDD.Infrastructure.Contexts;
using CarDDD.Infrastructure.Publisher.Rabbit;
using CarDDD.Infrastructure.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarDDD.Infrastructure.Extensions;

public static class StorageDependencyInjection
{
    public static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services.AddScoped<ITemporaryPhotoStorage, MemoryTemporaryPhotoStorage>();
        services.AddSingleton<IMainPhotoStorage, MinioPhotoStorage>();

        services.AddScoped<IPhotoStorage, MinioPhotoStorage>();

        services.AddScoped<IApplicationUserStorage, KeycloakApplicationUserStorage>();

        services.AddScoped<ICarSnapshotStorage, PostgresCarStorage>();

        services.AddSingleton<ICartSnapshotStorage, MemoryTemporaryCartStorage>();
        
        return services;
    }
}

public static class PublisherDependencyInjection
{
    public static IServiceCollection AddPublishers(this IServiceCollection services)
    {
        services.AddSingleton<RabbitRouteResolver>();

        services.AddSingleton<IIntegrationPublisher, RabbitPublisher>();
        
        return services;
    }
}

public static class ApplicationDbContextDependencyInjection
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>((_, opt) =>
        {
            opt.UseNpgsql(connectionString, npg =>
            {
                npg.EnableRetryOnFailure();
                npg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });
        
        return services;
    }
}