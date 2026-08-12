using BOMManagement.Application.Common.Persistence;
using BOMManagement.DataAccess.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BOMManagement.DataAccess.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
        // Register Generic Repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
