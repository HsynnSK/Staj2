using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.Item;

public class ItemDetailDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public UnitType MainUnit { get; set; }
    public int? DefaultWarehouseId { get; set; }
    public string DefaultWarehouseCode { get; set; } = string.Empty;
    public string DefaultWarehouseName { get; set; } = string.Empty;
    public decimal MinimumStockLevel { get; set; }
    public decimal StockQuantity { get; set; }
    public int MaterialTypeId { get; set; }
    public string MaterialTypeCode { get; set; } = string.Empty;
    public string MaterialTypeName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
