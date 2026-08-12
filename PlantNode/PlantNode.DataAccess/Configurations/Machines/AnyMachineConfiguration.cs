using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Machines;

namespace PlantNode.DataAccess.Configurations.Machines
{
    public class AnyMachineConfiguration : IEntityTypeConfiguration<AnyMachine>
    {
        public void Configure(EntityTypeBuilder<AnyMachine> builder)
        {
            builder.HasBaseType<Machine>();
        }
    }
}
