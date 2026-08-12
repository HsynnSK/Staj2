namespace BOMManagement.Application.DTOs.Production;

public class ProductionCreateDto
{
    public string ProductionNo { get; set; } = string.Empty;
    public string? ParentProductionNo { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public decimal TargetQuantity { get; set; } = 1;
    public int? BOMHeaderId { get; set; }
}
