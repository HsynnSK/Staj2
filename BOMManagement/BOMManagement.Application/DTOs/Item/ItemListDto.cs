using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.Item;

public class ItemListDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public UnitType MainUnit { get; set; }
    public string DefaultWarehouseCode { get; set; } = string.Empty;
    public decimal MinimumStockLevel { get; set; }
    public decimal StockQuantity { get; set; }
    public string MaterialTypeName { get; set; } = string.Empty;
}
