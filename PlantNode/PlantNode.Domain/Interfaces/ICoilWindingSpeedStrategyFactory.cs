using PlantNode.Domain.Entities.Products;

namespace PlantNode.Domain.Interfaces;

public interface ICoilWindingSpeedStrategyFactory
{
    ICoilWindingSpeedStrategy GetStrategy(MaterialType? materialType);
}
