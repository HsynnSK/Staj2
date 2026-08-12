using PlantNode.Domain.Entities.Machines;

namespace PlantNode.Domain.Interfaces;

public interface ICoilWindingSpeedStrategy
{
    float CalculateSpeed(CoilWindingMachine machine);
}
