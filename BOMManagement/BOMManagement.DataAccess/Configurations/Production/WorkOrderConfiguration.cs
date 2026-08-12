using BOMManagement.Domain.Entities.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.Production;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("WorkOrders");

        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.WorkOrderNo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.WarehouseCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ManufacturingCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.WorkCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.WorkCenterCode)
            .HasMaxLength(50);

        builder.Property(x => x.SetupTime)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.RunTime)
            .HasPrecision(18, 2)
            .IsRequired();

        // 1-to-1 index constraint on ItemCode to ensure each item code only has exactly one Work Order record
        builder.HasIndex(x => x.ItemCode)
            .IsUnique();
    }
}
