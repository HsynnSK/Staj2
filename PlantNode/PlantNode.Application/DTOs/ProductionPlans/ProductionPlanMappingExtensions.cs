using System;
using System.Linq;
using PlantNode.Domain.Entities.Production;
using PlantNode.Domain.Enums;

namespace PlantNode.Application.DTOs.ProductionPlans
{
    public static class ProductionPlanMappingExtensions
    {
        public static ProductionPlanDto ToDto(this ProductionPlan plan)
        {
            if (plan == null) return null!;

            var dto = new ProductionPlanDto
            {
                Id = plan.Id,
                PlanCode = plan.PlanCode,
                Description = plan.Description,
                PlannedStartDate = plan.PlannedStartDate,
                PlannedEndDate = plan.PlannedEndDate,
                EstimatedEndDate = plan.EstimatedEndDate,
                Status = plan.Status,
                StatusLabel = plan.Status switch
                {
                    ProductionPlanStatus.Demand => "Talep",
                    ProductionPlanStatus.Scheduled => "Planlandı",
                    ProductionPlanStatus.InProgress => "Üretimde",
                    ProductionPlanStatus.Completed => "Tamamlandı",
                    ProductionPlanStatus.NotCompleted => "Tamamlanamadı",
                    ProductionPlanStatus.Awaiting => "Bekleniyor",
                    _ => "Bilinmeyen"
                },
                TargetQuantity = plan.TargetQuantity
            };

            if (plan.PlanProducts != null)
            {
                dto.Products = plan.PlanProducts
                    .Where(pp => pp.Product != null)
                    .Select(pp => new ProductDto
                    {
                        Id = pp.Product!.Id,
                        Name = pp.Product.Name ?? string.Empty,
                        ProductCode = pp.Product.ProductCode ?? string.Empty
                    })
                    .ToList();
            }

            if (plan.PlanMachines != null)
            {
                dto.Machines = plan.PlanMachines
                    .Where(pm => pm.Machine != null)
                    .Select(pm => new MachineDto
                    {
                        Id = pm.Machine!.Id,
                        Name = pm.Machine.Name ?? string.Empty,
                        MachineCode = pm.Machine.MachineCode ?? string.Empty,
                        Status = pm.Machine.Status
                    })
                    .ToList();
            }

            return dto;
        }
    }
}
