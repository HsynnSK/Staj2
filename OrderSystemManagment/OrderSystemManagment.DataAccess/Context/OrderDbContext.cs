using Microsoft.EntityFrameworkCore;
using OrderSystemManagment.Domain.Entities;
using OrderSystemManagment.Domain.Entities.BaseModels;
using System.Linq.Expressions;

namespace OrderSystemManagment.DataAccess.Context;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base (options)
    {
       
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<CurrencyType> CurrencyTypes => Set<CurrencyType>();

    public DbSet<Item> Items => Set<Item>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteFilters(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Code);

            entity.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey("WarehouseId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey("CustomerId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property<int>("CurrencyTypeId");
            entity.HasOne(x => x.CurrencyType)
                .WithMany()
                .HasForeignKey("CurrencyTypeId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasOne(x => x.Order)
                .WithOne(x => x.Invoice)
                .HasForeignKey<Invoice>("OrderId")
                .HasPrincipalKey<Order>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property<int>("CurrencyTypeId");
            entity.HasOne(x => x.CurrencyType)
                .WithMany()
                .HasForeignKey("CurrencyTypeId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.ExchangeRate).HasPrecision(18, 4);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property<int>("ItemId");
            entity.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey("ItemId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property<int>("CurrencyTypeId");
            entity.HasOne(x => x.CurrencyType)
                .WithMany()
                .HasForeignKey("CurrencyTypeId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Order)
                .WithMany(x => x.OrderItems)
                .HasForeignKey("OrderId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.UnitPrice).HasPrecision(18, 2);
            entity.ToTable(table => table.HasCheckConstraint("CK_OrderItem_Quantity_Minimum", "[Quantity] >= 1"));
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.OwnsOne(x => x.UserManagement, nav =>
            {
                nav.Property(p => p.CanRead).HasColumnName("UserManagement_CanRead");
                nav.Property(p => p.CanCreate).HasColumnName("UserManagement_CanCreate");
                nav.Property(p => p.CanUpdate).HasColumnName("UserManagement_CanUpdate");
                nav.Property(p => p.CanDelete).HasColumnName("UserManagement_CanDelete");
            });

            entity.OwnsOne(x => x.OrderManagement, nav =>
            {
                nav.Property(p => p.CanRead).HasColumnName("OrderManagement_CanRead");
                nav.Property(p => p.CanCreate).HasColumnName("OrderManagement_CanCreate");
                nav.Property(p => p.CanUpdate).HasColumnName("OrderManagement_CanUpdate");
                nav.Property(p => p.CanDelete).HasColumnName("OrderManagement_CanDelete");
            });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property<int>("RoleId");
            entity.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey("RoleId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(DatabaseObject).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(DatabaseObject.Id)).HasColumnOrder(0);
                
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(DatabaseObject.CreatedDate)).HasColumnOrder(100);
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(DatabaseObject.UpdatedDate)).HasColumnOrder(101);
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(DatabaseObject.CreatedBy)).HasColumnOrder(102);
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(DatabaseObject.UpdatedBy)).HasColumnOrder(103);
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(DatabaseObject.IsDeleted)).HasColumnOrder(104);
            }
        }
    }

    public override int SaveChanges()
    {
        OnBeforeSaving();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is DatabaseObject && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (DatabaseObject)entry.Entity;
            var now = DateTime.Now;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = now;
                entity.CreatedBy = 1;
                entity.UpdatedDate = now;
                entity.UpdatedBy = 1;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(DatabaseObject.CreatedDate)).IsModified = false;
                entry.Property(nameof(DatabaseObject.CreatedBy)).IsModified = false;

                entity.UpdatedDate = now;
                entity.UpdatedBy = 1;
            }
        }
    }

    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(DatabaseObject).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var property = Expression.Property(parameter, nameof(DatabaseObject.IsDeleted));
            var body = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(body, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}