using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.BOM;

public class BOMHeaderConfiguration : IEntityTypeConfiguration<BOMHeader>
{
    public void Configure(EntityTypeBuilder<BOMHeader> builder)
    {
        builder.ToTable("BOMHeaders");

        // Primary Key & Soft Delete Query Filter
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.ParentItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.BOMCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.BaseQuantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.UnitCode)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        // 1-N relationship with BOMLine (Cascade delete)
        builder.HasMany(x => x.Lines)
            .WithOne(x => x.BOMHeader)
            .OnDelete(DeleteBehavior.Cascade);

        // 1-N relationship with BOMRoute (Cascade delete)
        builder.HasMany(x => x.Routes)
            .WithOne(x => x.BOMHeader)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
