using BOMManagement.Domain.Entities.BaseModels;
using BOMManagement.Domain.Enums;

namespace BOMManagement.Domain.Entities.Production;

public class PurchaseOrder : DatabaseObject
{
    public string PurchaseOrderNo { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? RelatedProductionNo { get; set; }
    public string? Notes { get; set; }
    public PurchaseOrderStatus Status { get; set; }
}