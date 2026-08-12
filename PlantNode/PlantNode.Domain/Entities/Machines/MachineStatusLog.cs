using PlantNode.Domain.Entities.BaseModels;
using PlantNode.Domain.Entities.Machines;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantNode.Domain.Enums;

public class MachineStatusLog : DatabaseObject
{

    public Machine? Machine { get; set; } // MachineId

    public string? Description { get; set; }

    public MachineStatus Status { get; set; }

    public DateTime StartTime { get; set; } = DateTime.Now;

    public DateTime? EndTime { get; set; }

    public int WeekNumber { get; set; }

    public int Year { get; set; }

}
