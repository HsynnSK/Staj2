using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.Production;

public class ProductionListDto
{
    public int Id { get; set; }
    public string ProductionNo { get; set; } = string.Empty;
    public string? ParentProductionNo { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal TargetQuantity { get; set; }
    public ProductionStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? BOMHeaderId { get; set; }
}
