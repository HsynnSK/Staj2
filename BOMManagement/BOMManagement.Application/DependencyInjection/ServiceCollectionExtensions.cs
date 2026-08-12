using System.Reflection;
using BOMManagement.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BOMManagement.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register AutoMapper profiles
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(BOMManagement.Application.Mappings.MappingProfile).Assembly));

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // Register Business Services
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IMaterialTypeService, MaterialTypeService>();
        services.AddScoped<IBOMService, BOMService>();
        services.AddScoped<IWorkOrderService, WorkOrderService>();
        services.AddScoped<IProductionService, ProductionService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

        return services;
    }
}
