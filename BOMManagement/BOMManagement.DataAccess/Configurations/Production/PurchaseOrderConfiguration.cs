using BOMManagement.Domain.Entities.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.Production;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.PurchaseOrderNo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.RelatedProductionNo)
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        // Unique Index on PurchaseOrderNo
        builder.HasIndex(x => x.PurchaseOrderNo)
            .IsUnique();
    }
}
