using System;
using PlantNode.Domain.Entities.BaseModels;
using PlantNode.Domain.Entities.Machines;

namespace PlantNode.Domain.Entities.Production
{
    public class ProductionPlanMachine : DatabaseObject
    {
        public virtual ProductionPlan? ProductionPlan { get; set; }//id
        public virtual Machine? Machine { get; set; }//id
    }
}
