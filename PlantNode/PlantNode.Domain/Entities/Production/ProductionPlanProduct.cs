using System;
using PlantNode.Domain.Entities.BaseModels;
using PlantNode.Domain.Entities.Products;

namespace PlantNode.Domain.Entities.Production
{
    public class ProductionPlanProduct : DatabaseObject
    {
        public virtual ProductionPlan? ProductionPlan { get; set; }//id
        public virtual Product? Product { get; set; }//id
    }
}
