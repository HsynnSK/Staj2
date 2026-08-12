using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.BOM;

public class BOMLineDto
{
    public int Id { get; set; }
    public string ChildItemCode { get; set; } = string.Empty;
    public string ChildItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitType UnitCode { get; set; }
    public decimal ScrapRate { get; set; }
    public string ConsumptionWarehouse { get; set; } = string.Empty;
    public bool IsSubAssembly { get; set; }
}
