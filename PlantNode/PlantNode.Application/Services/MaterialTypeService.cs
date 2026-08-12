using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantNode.Application.Interfaces;
using PlantNode.Domain.Entities.Products;

namespace PlantNode.Application.Services
{
    public class MaterialTypeService : IMaterialTypeService
    {
        private readonly IPlantNodeDbContext _dbContext;

        public MaterialTypeService(IPlantNodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MaterialType>> GetAllMaterialTypesAsync()
        {
            return await _dbContext.MaterialTypes
                .Where(mt => !mt.IsDeleted)
                .OrderBy(mt => mt.Name)
                .ToListAsync();
        }

        public async Task<MaterialType?> GetMaterialTypeByIdAsync(int id)
        {
            return await _dbContext.MaterialTypes
                .FirstOrDefaultAsync(mt => mt.Id == id && !mt.IsDeleted);
        }

        public async Task SaveMaterialTypeAsync(MaterialType materialType)
        {
            if (materialType.Id == 0)
            {
                materialType.CreatedDate = DateTime.Now;
                materialType.UpdatedDate = DateTime.Now;
                _dbContext.MaterialTypes.Add(materialType);
            }
            else
            {
                materialType.UpdatedDate = DateTime.Now;
                _dbContext.MaterialTypes.Update(materialType);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMaterialTypeAsync(int id)
        {
            var materialType = await GetMaterialTypeByIdAsync(id);
            if (materialType != null)
            {
                materialType.IsDeleted = true;
                materialType.UpdatedDate = DateTime.Now;
                _dbContext.MaterialTypes.Update(materialType);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
