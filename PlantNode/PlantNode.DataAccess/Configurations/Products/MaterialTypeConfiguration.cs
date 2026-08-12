using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Products;

namespace PlantNode.DataAccess.Configurations.Products;

public class MaterialTypeConfiguration : IEntityTypeConfiguration<MaterialType>
{
    public void Configure(EntityTypeBuilder<MaterialType> builder)
    {
        builder.ToTable("MaterialTypes");

        builder.HasKey(mt => mt.Id);

        builder.Property(mt => mt.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mt => mt.MaterialCode)
            .HasMaxLength(50);

        builder.Property(mt => mt.MaxMachineSpeed)
            .IsRequired();

        builder.Property(mt => mt.TensionLimit)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(mt => mt.AccelerationRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
