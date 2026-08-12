using BOMManagement.Domain.Entities.BaseModels;
using BOMManagement.Domain.Enums;

namespace BOMManagement.Domain.Entities.BOMs;

public class BOMLine : DatabaseObject
{
    public string ChildItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitType UnitCode { get; set; }
    public decimal ScrapRate { get; set; }
    public string ConsumptionWarehouse { get; set; } = string.Empty;
    public bool IsSubAssembly { get; set; }

    public virtual BOMHeader BOMHeader { get; set; } = null!; //BOMHeader
    public virtual BOMRoute? BOMRoute { get; set; } //OperationSeq
}
