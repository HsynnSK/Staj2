using OrderSystemManagment.Domain.Entities.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystemManagment.Domain.Entities;

public class Warehouse : DatabaseObject
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}
