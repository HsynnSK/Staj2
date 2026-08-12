using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Plants;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantNode.DataAccess.Configurations.Plants
{
    public class CoildWindingPlantConfiguration: IEntityTypeConfiguration<CoilWindingPlant>
    {
        public void Configure(EntityTypeBuilder<CoilWindingPlant> builder)
        {
            builder.HasBaseType<Plant>();

            builder.Property(p => p.MaxSupportedWireGauge)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder.Property(p => p.MaxAnnealingTemperatureCelsius)
                .IsRequired();
        }
    }
}
