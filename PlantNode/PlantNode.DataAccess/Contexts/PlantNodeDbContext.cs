using Microsoft.EntityFrameworkCore;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Entities.Plants;
using PlantNode.Domain.Entities.Products;
using PlantNode.Domain.Entities.Production;
using PlantNode.Domain.Enums;
using PlantNode.Application.Interfaces;

namespace PlantNode.DataAccess.Contexts
{
    public class PlantNodeDbContext : DbContext, IPlantNodeDbContext
    {
        public PlantNodeDbContext(DbContextOptions<PlantNodeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Plant> Plants { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<MachineStatusLog> MachineStatusLogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<ProductionPlan> ProductionPlans { get; set; }
        public DbSet<ProductionPlanProduct> ProductionPlanProducts { get; set; }
        public DbSet<ProductionPlanMachine> ProductionPlanMachines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlantNodeDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
