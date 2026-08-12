using PlantNode.Domain.Entities.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantNode.Domain.Entities.Products;

public class MaterialType : DatabaseObject
{
    public string Name { get; set; } = string.Empty;
    public string? MaterialCode { get; set; }

    public float MaxMachineSpeed { get; set; }
    public decimal TensionLimit { get; set; }
    public decimal AccelerationRate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
