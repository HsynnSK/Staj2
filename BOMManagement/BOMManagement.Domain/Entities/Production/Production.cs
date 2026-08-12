using BOMManagement.Domain.Entities.BaseModels;
using BOMManagement.Domain.Enums;

namespace BOMManagement.Domain.Entities.Production;

public class Production : DatabaseObject
{
    public string ProductionNo { get; set; } = string.Empty;
    public string? ParentProductionNo { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public decimal TargetQuantity { get; set; }
    public ProductionStatus Status { get; set; }
    public int? BOMHeaderId { get; set; }
}
