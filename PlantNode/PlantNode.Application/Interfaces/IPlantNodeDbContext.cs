using Microsoft.EntityFrameworkCore;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Entities.Plants;
using PlantNode.Domain.Entities.Products;
using PlantNode.Domain.Entities.Production;
using PlantNode.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace PlantNode.Application.Interfaces
{
    public interface IPlantNodeDbContext
    {
        DbSet<Plant> Plants { get; }
        DbSet<Machine> Machines { get; }
        DbSet<MachineStatusLog> MachineStatusLogs { get; }
        DbSet<Product> Products { get; }
        DbSet<MaterialType> MaterialTypes { get; }
        DbSet<ProductionPlan> ProductionPlans { get; }
        DbSet<ProductionPlanProduct> ProductionPlanProducts { get; }
        DbSet<ProductionPlanMachine> ProductionPlanMachines { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
