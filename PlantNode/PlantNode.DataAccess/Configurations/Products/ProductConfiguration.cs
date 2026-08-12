using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Products;

namespace PlantNode.DataAccess.Configurations.Products
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(p => p.ProductCode)
                .HasMaxLength(50);
            
            builder.Property(p => p.Barcode)
                .HasMaxLength(50);
            
            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.HasOne(mt => mt.MaterialType)
                .WithMany(p => p.Products)
                .HasForeignKey("MaterialTypeId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
