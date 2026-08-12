using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantNode.Domain.Entities.Production;

namespace PlantNode.Application.Interfaces
{
    public interface IProductionPlanService
    {
        Task<List<ProductionPlan>> GetAllProductionPlansAsync();
        Task<ProductionPlan?> GetProductionPlanByIdAsync(int id);
        Task SaveProductionPlanAsync(ProductionPlan plan);
        Task DeleteProductionPlanAsync(int id);
        Task SyncAllPlanStatusesAsync();
    }
}
