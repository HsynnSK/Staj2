using System;
using PlantNode.Domain.Enums;

namespace PlantNode.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetLabel(this MachineStatus status) => status switch
    {
        MachineStatus.Active => "Aktif",
        MachineStatus.Idle => "Boşta",
        MachineStatus.Maintenance => "Bakımda",
        MachineStatus.Malfunction => "Arızalı",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public static string GetCssClass(this MachineStatus status) => status switch
    {
        MachineStatus.Active => "active",
        MachineStatus.Idle => "idle",
        MachineStatus.Maintenance => "maintenance",
        MachineStatus.Malfunction => "malfunction",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };
}
