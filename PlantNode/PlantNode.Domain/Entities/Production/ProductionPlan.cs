using System;
using System.Collections.Generic;
using PlantNode.Domain.Entities.BaseModels;
using PlantNode.Domain.Enums;

namespace PlantNode.Domain.Entities.Production
{
    public class ProductionPlan : DatabaseObject
    {
        public string PlanCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public ProductionPlanStatus Status { get; set; } = ProductionPlanStatus.Scheduled;
        public int TargetQuantity { get; set; } = 100;
        public DateTime EstimatedEndDate { get; set; } = DateTime.Now;

        // Navigation properties for explicit join entities
        public virtual ICollection<ProductionPlanProduct> PlanProducts { get; set; } = new List<ProductionPlanProduct>();
        public virtual ICollection<ProductionPlanMachine> PlanMachines { get; set; } = new List<ProductionPlanMachine>();
    }
}
