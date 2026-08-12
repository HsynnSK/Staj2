using BOMManagement.Domain.Entities.Items;
using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Entities.Production;
using BOMManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.Inventory;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ItemName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ItemType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.MainUnit)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(x => x.DefaultWarehouse)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.MinimumStockLevel)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.StockQuantity)
            .HasPrecision(18, 4)
            .IsRequired()
            .HasDefaultValue(0.0000);

        // Unique Index on ItemCode
        builder.HasIndex(x => x.ItemCode)
            .IsUnique();

        // 1-N relationship with MaterialType (via Shadow Foreign Key)
        builder.HasOne(x => x.MaterialType)
            .WithMany(x => x.Items)
            .OnDelete(DeleteBehavior.Restrict);

        // 1-N relationship with BOMHeader (using ItemCode as Principal Key and ParentItemCode as Foreign Key)
        builder.HasMany(x => x.Recipes)
            .WithOne()
            .HasForeignKey(x => x.ParentItemCode)
            .HasPrincipalKey(x => x.ItemCode)
            .OnDelete(DeleteBehavior.Restrict);

        // 1-1 relationship with WorkOrder (ItemCode as Principal Key, and WorkOrder has ItemCode as Foreign Key)
        builder.HasOne(x => x.WorkOrder)
            .WithOne()
            .HasForeignKey<WorkOrder>(x => x.ItemCode)
            .HasPrincipalKey<Item>(x => x.ItemCode)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
