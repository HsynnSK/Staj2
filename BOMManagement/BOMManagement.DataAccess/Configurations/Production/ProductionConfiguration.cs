using ProductionEntity = BOMManagement.Domain.Entities.Production.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.ProductionMapping;

public class ProductionConfiguration : IEntityTypeConfiguration<ProductionEntity>
{
    public void Configure(EntityTypeBuilder<ProductionEntity> builder)
    {
        builder.ToTable("Productions");

        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.ProductionNo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ParentProductionNo)
            .HasMaxLength(50);

        builder.Property(x => x.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TargetQuantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.BOMHeaderId)
            .IsRequired(false);

        // Unique Index on ProductionNo
        builder.HasIndex(x => x.ProductionNo)
            .IsUnique();
    }
}
