using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.BOM;

public class BOMUsageDto
{
    public int BOMHeaderId { get; set; }
    public string BOMCode { get; set; } = string.Empty;
    public int Version { get; set; }
    public string ParentItemCode { get; set; } = string.Empty;
    public string ParentItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitType UnitCode { get; set; }
    public decimal ScrapRate { get; set; }
    public string ConsumptionWarehouse { get; set; } = string.Empty;
}
