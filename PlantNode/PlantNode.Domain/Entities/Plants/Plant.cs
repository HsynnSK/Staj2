using PlantNode.Domain.Entities.BaseModels;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantNode.Domain.Entities.Plants;

public abstract class Plant : DatabaseObject
{
    public string? Name { get; set; }
    public string PlantCode { get; set; } = string.Empty;

    public PlantType Type { get; set; }// 1 = Manufacturing, 2 = Assembly, 3 = Packaging, 4 = Warehouse, 5 = Distribution

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
}
