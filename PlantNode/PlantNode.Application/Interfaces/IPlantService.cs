using System.Collections.Generic;
using System.Threading.Tasks;
using PlantNode.Domain.Entities.Plants;

namespace PlantNode.Application.Interfaces;

public interface IPlantService
{
    Task<List<Plant>> GetAllActivePlantsAsync();
    Task<Plant?> GetPlantByIdAsync(int id);
    Task SavePlantAsync(Plant plant);
    Task DeletePlantAsync(int id);
    Task<Dictionary<int, int>> GetPlantMachineCountsAsync();
}
