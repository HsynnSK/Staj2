using PlantNode.Domain.Entities.Products;
using PlantNode.Domain.Interfaces;

namespace PlantNode.Domain.Strategies;

public class CoilWindingSpeedStrategyFactory : ICoilWindingSpeedStrategyFactory
{
    private readonly ICoilWindingSpeedStrategy _dynamicStrategy = new MaterialTypeSpeedStrategy();

    public ICoilWindingSpeedStrategy GetStrategy(MaterialType? materialType)
    {
        return _dynamicStrategy;
    }
}
