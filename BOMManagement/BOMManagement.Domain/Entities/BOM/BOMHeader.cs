using BOMManagement.Domain.Entities.BaseModels;
using BOMManagement.Domain.Enums;

namespace BOMManagement.Domain.Entities.BOMs;

public class BOMHeader : DatabaseObject
{
    public string ParentItemCode { get; set; } = string.Empty;
    public string BOMCode { get; set; } = string.Empty;
    public int Version { get; set; }
    public decimal BaseQuantity { get; set; }
    public UnitType UnitCode { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<BOMLine>? Lines { get; set; }
    public virtual ICollection<BOMRoute>? Routes { get; set; }
}
