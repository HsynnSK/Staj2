using PlantNode.Domain.Entities.BaseModels;
using PlantNode.Domain.Entities.Plants;
using PlantNode.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantNode.Domain.Entities.Machines;

public abstract class Machine : DatabaseObject
{
    public string? Name { get; set; }

    public string MachineCode { get; set; } = string.Empty;

    public Plant? Plant { get; set; }// PlantId

    public string? SerialNumber { get; set; }

    public bool IsActive { get; set; } = false;

    public float Width { get; set; }

    public float Height { get; set; }

    public float Depth { get; set; }

    public MachineStatus Status { get; set; }// 1 = Inactive, 2 = Active, 3 = Idle, 4 = Maintenance, 5 = Malfunction

}
