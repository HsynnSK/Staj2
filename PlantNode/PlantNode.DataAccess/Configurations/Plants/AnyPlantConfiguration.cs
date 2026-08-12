using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Plants;

namespace PlantNode.DataAccess.Configurations.Plants
{
    public class AnyPlantConfiguration : IEntityTypeConfiguration<AnyPlant>
    {
        public void Configure(EntityTypeBuilder<AnyPlant> builder)
        {
            builder.HasBaseType<Plant>();
        }
    }
}
