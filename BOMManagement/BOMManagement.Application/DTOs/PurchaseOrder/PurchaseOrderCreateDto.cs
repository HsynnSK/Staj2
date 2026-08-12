using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.PurchaseOrder;

public class PurchaseOrderCreateDto
{
    public string PurchaseOrderNo { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? RelatedProductionNo { get; set; }
    public string? Notes { get; set; }
}
