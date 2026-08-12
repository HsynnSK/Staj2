using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Plants;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PlantNode.DataAccess.Configurations.Plants
{
    public class PlantConfiguration : IEntityTypeConfiguration<Plant>
    {
        public void Configure(EntityTypeBuilder<Plant> builder)
        {
            builder.ToTable("Plants");
            
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(p => p.PlantCode)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(p => p.Type)
                .IsRequired();
        }
    }
}
