using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Entities;

namespace PlantNode.DataAccess.Configurations.Machines
{
    public class CoilWindingMachineConfiguration : IEntityTypeConfiguration<CoilWindingMachine>
    {
        public void Configure(EntityTypeBuilder<CoilWindingMachine> builder)
        {
            builder.HasBaseType<Machine>();

            builder.Property(c => c.MaxRpm)
                .IsRequired();

            builder.Property(c => c.MinWireDiameter)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.MaxWireDiameter)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.MaxCoilDiameter)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.SpindleCount)
                .IsRequired();

            builder.Property(c => c.TargetTurnCount)
                .IsRequired();

            builder.Property(c => c.CurrentTurnCount)
                .IsRequired();

            builder.Property(c => c.WireTension)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.TargetSpeed)
                .IsRequired();

            builder.HasOne(c => c.CurrentProduct)
                .WithMany()
                .HasForeignKey(c => c.CurrentProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
