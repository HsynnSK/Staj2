using BOMManagement.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BOMManagement.DataAccess.Configurations.Inventory;

public class MaterialTypeConfiguration : IEntityTypeConfiguration<MaterialType>
{
    public void Configure(EntityTypeBuilder<MaterialType> builder)
    {
        builder.ToTable("MaterialTypes");

        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Unique Index on Code
        builder.HasIndex(x => x.Code)
            .IsUnique();

        // Seed some initial material types
        builder.HasData(
            new MaterialType
            {
                Id = 1,
                Code = "MT-PLASTIK",
                Name = "Plastik Malzeme",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            },
            new MaterialType
            {
                Id = 2,
                Code = "MT-METAL",
                Name = "Metal Malzeme",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            },
            new MaterialType
            {
                Id = 3,
                Code = "MT-ELEKTRONIK",
                Name = "Elektronik Bileşen",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            },
            new MaterialType
            {
                Id = 4,
                Code = "MT-MONTAJ",
                Name = "Montaj Bileşeni",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            },
            new MaterialType
            {
                Id = 5,
                Code = "MT-MEKANIK",
                Name = "Mekanik Bileşen",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            },
            new MaterialType
            {
                Id = 6,
                Code = "MT-MAMUL",
                Name = "Mamul Ürün",
                CreatedDate = new DateTime(2026, 1, 1),
                UpdatedDate = new DateTime(2026, 1, 1),
                IsDeleted = false
            }
        );
    }
}
