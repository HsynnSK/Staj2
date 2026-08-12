using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Enums;

namespace PlantNode.Application.Interfaces;

public interface IMaintenanceService
{
    Task<List<MachineStatusLog>> GetAllActiveLogsAsync();
    Task<MachineStatusLog?> GetLogByIdAsync(int id);
    Task SaveLogAsync(MachineStatusLog log);
    Task DeleteLogAsync(int id);
    Task<bool> HasStatusOverlapAsync(int machineId, DateTime start, DateTime end, int? excludeLogId = null);
    Task<List<MachineStatusLog>> GetRecentStatusLogsAsync(int count);
    Task SyncAllMachineStatusesAsync();
}
