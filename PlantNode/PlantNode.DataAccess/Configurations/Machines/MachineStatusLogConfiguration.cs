using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Enums;

namespace PlantNode.DataAccess.Configurations.Machines
{
    public class MachineStatusLogConfiguration : IEntityTypeConfiguration<MachineStatusLog>
    {
        public void Configure(EntityTypeBuilder<MachineStatusLog> builder)
        {
            builder.HasKey(msl => msl.Id);
            
            builder.Property(msl => msl.Description)
                .HasMaxLength(500);
            
            builder.Property(msl => msl.Status)
                .IsRequired();
            
            builder.Property(msl => msl.StartTime)
                .IsRequired();
            
            builder.Property(msl => msl.WeekNumber)
                .IsRequired();
            
            builder.Property(msl => msl.Year)
                .IsRequired();
            
            builder.HasOne(msl => msl.Machine)
                .WithMany()
                .HasForeignKey("MachineId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
