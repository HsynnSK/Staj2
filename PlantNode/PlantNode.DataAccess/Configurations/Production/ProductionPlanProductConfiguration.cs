using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantNode.Domain.Entities.Production;

namespace PlantNode.DataAccess.Configurations.Production
{
    public class ProductionPlanProductConfiguration : IEntityTypeConfiguration<ProductionPlanProduct>
    {
        public void Configure(EntityTypeBuilder<ProductionPlanProduct> builder)
        {
            builder.HasKey(pp => pp.Id);

            builder.HasOne(pp => pp.Product)
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
