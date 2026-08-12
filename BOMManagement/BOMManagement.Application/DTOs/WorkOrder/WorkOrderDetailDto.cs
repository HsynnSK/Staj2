namespace BOMManagement.Application.DTOs.WorkOrder;

public class WorkOrderDetailDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string WorkOrderNo { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string ManufacturingCode { get; set; } = string.Empty;
    public string WorkCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? WorkCenterCode { get; set; }
    public decimal SetupTime { get; set; }
    public decimal RunTime { get; set; }
}
