using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.BOM;

public class BOMHeaderCreateDto
{
    public string ParentItemCode { get; set; } = string.Empty;
    public string BOMCode { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public decimal BaseQuantity { get; set; } = 1;
    public UnitType UnitCode { get; set; } = UnitType.Piece;
    public bool IsActive { get; set; } = true;

    public List<BOMLineDto> Lines { get; set; } = [];
}
