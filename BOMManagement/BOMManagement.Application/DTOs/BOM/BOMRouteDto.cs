namespace BOMManagement.Application.DTOs.BOM;

public class BOMRouteDto
{
    public int Id { get; set; }
    public int OperationSeq { get; set; }
    public string WorkCenterCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public decimal SetupTime { get; set; }
    public decimal RunTime { get; set; }
}
