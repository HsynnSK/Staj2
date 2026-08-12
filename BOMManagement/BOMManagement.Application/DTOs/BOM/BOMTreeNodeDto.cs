using System.Collections.Generic;

namespace BOMManagement.Application.DTOs.BOM;

public class BOMTreeNodeDto
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string ItemTypeDescription { get; set; } = string.Empty;
    public bool IsSubAssembly { get; set; }
    public string ConsumptionWarehouse { get; set; } = string.Empty;
    public decimal StockQuantity { get; set; }
    public List<string> ProductionNumbers { get; set; } = [];
    public List<BOMTreeNodeDto> Children { get; set; } = [];
}
