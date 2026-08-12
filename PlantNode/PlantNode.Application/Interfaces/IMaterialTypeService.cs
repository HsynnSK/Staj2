using System.Collections.Generic;
using System.Threading.Tasks;
using PlantNode.Domain.Entities.Products;

namespace PlantNode.Application.Interfaces;

public interface IMaterialTypeService
{
    Task<List<MaterialType>> GetAllMaterialTypesAsync();
    Task<MaterialType?> GetMaterialTypeByIdAsync(int id);
    Task SaveMaterialTypeAsync(MaterialType materialType);
    Task DeleteMaterialTypeAsync(int id);
}
