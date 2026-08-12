using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Production;

namespace PlantNode.DataAccess.Configurations.Production
{
    public class ProductionPlanMachineConfiguration : IEntityTypeConfiguration<ProductionPlanMachine>
    {
        public void Configure(EntityTypeBuilder<ProductionPlanMachine> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.HasOne(pm => pm.Machine)
                .WithMany()
                .HasForeignKey("MachineId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
