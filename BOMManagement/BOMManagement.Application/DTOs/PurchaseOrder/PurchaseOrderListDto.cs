using BOMManagement.Domain.Enums;
using System;

namespace BOMManagement.Application.DTOs.PurchaseOrder;

public class PurchaseOrderListDto
{
    public int Id { get; set; }
    public string PurchaseOrderNo { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? RelatedProductionNo { get; set; }
    public string? Notes { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
}
