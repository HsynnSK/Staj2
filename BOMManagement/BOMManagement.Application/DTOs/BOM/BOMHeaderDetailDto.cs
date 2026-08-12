using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.BOM;

public class BOMHeaderDetailDto
{
    public int Id { get; set; }
    public string ParentItemCode { get; set; } = string.Empty;
    public string ParentItemName { get; set; } = string.Empty;
    public string BOMCode { get; set; } = string.Empty;
    public int Version { get; set; }
    public decimal BaseQuantity { get; set; }
    public UnitType UnitCode { get; set; }
    public bool IsActive { get; set; }

    public List<BOMLineDto> Lines { get; set; } = [];
}
