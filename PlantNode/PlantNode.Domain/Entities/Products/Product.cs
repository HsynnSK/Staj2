using PlantNode.Domain.Entities.BaseModels;
using PlantNode.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace PlantNode.Domain.Entities.Products;

public class Product : DatabaseObject
{
    public string Name { get; set; } = string.Empty;

    public string? ProductCode { get; set; }

    public string? Barcode { get; set; }

    public string? Description { get; set; }

    public MaterialType? MaterialType { get; set; }
}
