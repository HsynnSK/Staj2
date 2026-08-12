using BOMManagement.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.Inventory;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.WarehouseCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.WarehouseName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.WarehouseCode)
            .IsUnique();

        // Seed some initial warehouses
        builder.HasData(
            new Warehouse
            {
                Id = 1,
                WarehouseCode = "WH-01",
                WarehouseName = "Ana Depo",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            },
            new Warehouse
            {
                Id = 2,
                WarehouseCode = "WH-02",
                WarehouseName = "Hammadde Deposu",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            },
            new Warehouse
            {
                Id = 3,
                WarehouseCode = "WH-03",
                WarehouseName = "Yarı Mamül Deposu",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            }
        );
    }
}
