using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Production;

namespace PlantNode.DataAccess.Configurations.Production
{
    public class ProductionPlanConfiguration : IEntityTypeConfiguration<ProductionPlan>
    {
        public void Configure(EntityTypeBuilder<ProductionPlan> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PlanCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            // Configure relationships using explicit join entities (which have shadow FKs)
            builder.HasMany(p => p.PlanProducts)
                .WithOne(pp => pp.ProductionPlan)
                .HasForeignKey("ProductionPlanId") // Shadow Foreign Key
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PlanMachines)
                .WithOne(pm => pm.ProductionPlan)
                .HasForeignKey("ProductionPlanId") // Shadow Foreign Key
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
