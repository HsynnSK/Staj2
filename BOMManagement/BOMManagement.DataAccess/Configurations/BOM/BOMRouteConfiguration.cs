using BOMManagement.Domain.Entities.BOMs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.BOM;

public class BOMRouteConfiguration : IEntityTypeConfiguration<BOMRoute>
{
    public void Configure(EntityTypeBuilder<BOMRoute> builder)
    {
        builder.ToTable("BOMRoutes");

        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.OperationSeq)
            .IsRequired();

        builder.Property(x => x.WorkCenterCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.OperationName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SetupTime)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.RunTime)
            .HasPrecision(18, 2)
            .IsRequired();

        // 1-N relationship with BOMHeader (Cascade delete, via Shadow Foreign Key)
        builder.HasOne(x => x.BOMHeader)
            .WithMany(x => x.Routes)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
