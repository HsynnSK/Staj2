using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.BOM;

public class BOMLineConfiguration : IEntityTypeConfiguration<BOMLine>
{
    public void Configure(EntityTypeBuilder<BOMLine> builder)
    {
        builder.ToTable("BOMLines");

        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.ChildItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.UnitCode)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ScrapRate)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.ConsumptionWarehouse)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IsSubAssembly)
            .IsRequired();

        // 1-N relationship with BOMHeader (Cascade delete, via Shadow Foreign Key)
        builder.HasOne(x => x.BOMHeader)
            .WithMany(x => x.Lines)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
