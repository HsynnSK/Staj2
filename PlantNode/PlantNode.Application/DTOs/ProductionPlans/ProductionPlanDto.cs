using System;
using System.Collections.Generic;
using PlantNode.Domain.Enums;

namespace PlantNode.Application.DTOs.ProductionPlans
{
    public class ProductionPlanDto
    {
        public int Id { get; set; }
        public string PlanCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public ProductionPlanStatus Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public int TargetQuantity { get; set; }
        public List<ProductDto> Products { get; set; } = new();
        public List<MachineDto> Machines { get; set; } = new();
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
    }

    public class MachineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MachineCode { get; set; } = string.Empty;
        public MachineStatus Status { get; set; }
    }
}
