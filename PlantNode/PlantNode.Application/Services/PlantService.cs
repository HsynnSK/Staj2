using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantNode.Application.Interfaces;
using PlantNode.Domain.Entities.Plants;

namespace PlantNode.Application.Services
{
    public class PlantService : IPlantService
    {
        private readonly IPlantNodeDbContext _dbContext;

        public PlantService(IPlantNodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Plant>> GetAllActivePlantsAsync()
        {
            return await _dbContext.Plants
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Plant?> GetPlantByIdAsync(int id)
        {
            return await _dbContext.Plants.FindAsync(id);
        }

        public async Task SavePlantAsync(Plant plant)
        {
            if (plant.Id == 0)
            {
                _dbContext.Plants.Add(plant);
            }
            else
            {
                _dbContext.Plants.Update(plant);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePlantAsync(int id)
        {
            var plant = await _dbContext.Plants.FindAsync(id);
            if (plant != null)
            {
                plant.IsDeleted = true;
                plant.UpdatedDate = System.DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<int, int>> GetPlantMachineCountsAsync()
        {
            return await _dbContext.Machines
                .Where(m => !m.IsDeleted)
                .GroupBy(m => EF.Property<int>(m, "PlantId"))
                .Select(g => new { PlantId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PlantId, x => x.Count);
        }
    }
}
