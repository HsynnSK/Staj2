using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Machines;

namespace PlantNode.DataAccess.Configurations.Machines
{
    public class MachineConfiguration : IEntityTypeConfiguration<Machine>
    {
        public void Configure(EntityTypeBuilder<Machine> builder)
        {
            builder.ToTable("Machines");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.MachineCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(m => m.Plant)
                .WithMany(p => p.Machines)
                .HasForeignKey("PlantId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(m => m.SerialNumber)
                .HasMaxLength(50);

            builder.Property(m => m.IsActive)
                .IsRequired();

            builder.Property(m => m.Width)
                .IsRequired();

            builder.Property(m => m.Height)
                .IsRequired();

            builder.Property(m => m.Depth)
                .IsRequired();

            builder.Property(m => m.Status)
                .IsRequired();
        }
    }
}
