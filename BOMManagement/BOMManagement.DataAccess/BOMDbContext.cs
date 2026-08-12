using BOMManagement.Domain.Entities;
using BOMManagement.Domain.Entities.BaseModels;
using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using BOMManagement.Domain.Entities.Production;

namespace BOMManagement.DataAccess;

public class BOMDbContext : DbContext
{
    public BOMDbContext(DbContextOptions<BOMDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<MaterialType> MaterialTypes { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<BOMHeader> BOMHeaders { get; set; } = null!;
    public DbSet<BOMLine> BOMLines { get; set; } = null!;
    public DbSet<BOMRoute> BOMRoutes { get; set; } = null!;
    public DbSet<Production> Productions { get; set; } = null!;
    public DbSet<WorkOrder> WorkOrders { get; set; } = null!;
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BOMDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var entries = ChangeTracker.Entries<DatabaseObject>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.UpdatedDate = now;
                
                // Assign a default system user (e.g., 1) if not explicitly set
                if (entry.Entity.CreatedBy == 0) entry.Entity.CreatedBy = 1;
                if (entry.Entity.UpdatedBy == 0) entry.Entity.UpdatedBy = 1;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedDate = now;
                
                if (entry.Entity.UpdatedBy == 0) entry.Entity.UpdatedBy = 1;
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Convert hard delete to soft delete
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.UpdatedDate = now;
            }
        }
    }
}
