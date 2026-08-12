using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantNode.Application.Interfaces;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Enums;
using PlantNode.Domain.Extensions;

namespace PlantNode.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IPlantNodeDbContext _dbContext;

        public MaintenanceService(IPlantNodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MachineStatusLog>> GetAllActiveLogsAsync()
        {
            return await _dbContext.MachineStatusLogs
                .Include(l => l.Machine)
                .Where(l => !l.IsDeleted)
                .OrderByDescending(l => l.StartTime)
                .ToListAsync();
        }

        public async Task<MachineStatusLog?> GetLogByIdAsync(int id)
        {
            return await _dbContext.MachineStatusLogs
                .Include(l => l.Machine)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task SaveLogAsync(MachineStatusLog log)
        {
            if (log.Machine != null)
            {
                var trackedMachine = await _dbContext.Machines.FindAsync(log.Machine.Id);
                if (trackedMachine != null)
                {
                    log.Machine = trackedMachine;
                }
            }

            if (log.Id == 0)
            {
                _dbContext.MachineStatusLogs.Add(log);
            }
            else
            {
                _dbContext.MachineStatusLogs.Update(log);
            }

            if (log.Machine != null && (log.Status == MachineStatus.Maintenance || log.Status == MachineStatus.Malfunction))
            {
                var ongoingPassiveLogs = await _dbContext.MachineStatusLogs
                    .Where(l => !l.IsDeleted && l.Machine != null && l.Machine.Id == log.Machine.Id && l.EndTime == null)
                    .Where(l => l.Status == MachineStatus.Active || l.Status == MachineStatus.Idle)
                    .Where(l => l.StartTime <= log.StartTime)
                    .ToListAsync();

                foreach (var ongoingLog in ongoingPassiveLogs)
                {
                    ongoingLog.EndTime = log.StartTime;
                    ongoingLog.UpdatedDate = DateTime.Now;
                }
            }
            else if (log.Machine != null && (log.Status == MachineStatus.Active || log.Status == MachineStatus.Idle))
            {
                var ongoingActiveLogs = await _dbContext.MachineStatusLogs
                    .Where(l => !l.IsDeleted && l.Machine != null && l.Machine.Id == log.Machine.Id && l.EndTime == null)
                    .Where(l => l.Status == MachineStatus.Maintenance || l.Status == MachineStatus.Malfunction)
                    .Where(l => l.StartTime <= log.StartTime)
                    .ToListAsync();

                foreach (var ongoingLog in ongoingActiveLogs)
                {
                    ongoingLog.EndTime = log.StartTime;
                    ongoingLog.UpdatedDate = DateTime.Now;
                }
            }

            await _dbContext.SaveChangesAsync();

            if (log.Machine != null)
            {
                await SyncMachineStatusFromLogsAsync(log.Machine.Id);
            }
        }

        public async Task DeleteLogAsync(int id)
        {
            var log = await _dbContext.MachineStatusLogs
                .Include(l => l.Machine)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (log != null)
            {
                log.IsDeleted = true;
                log.UpdatedDate = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                if (log.Machine != null)
                {
                    await SyncMachineStatusFromLogsAsync(log.Machine.Id);
                }
            }
        }

        public async Task<bool> HasStatusOverlapAsync(int machineId, DateTime start, DateTime end, int? excludeLogId = null)
        {
            var existingLogs = await _dbContext.MachineStatusLogs
                .Where(l => !l.IsDeleted && l.Machine != null && l.Machine.Id == machineId && l.Id != (excludeLogId ?? 0))
                .Where(l => l.Status == MachineStatus.Maintenance || l.Status == MachineStatus.Malfunction)
                .ToListAsync();

            foreach (var existingLog in existingLogs)
            {
                if (existingLog.EndTime.HasValue)
                {
                    if (start < existingLog.EndTime.Value && existingLog.StartTime < end)
                    {
                        return true;
                    }
                }
                else
                {
                    if (existingLog.StartTime <= start)
                    {
                        return true;
                    }
                    else
                    {
                        if (end > existingLog.StartTime)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public async Task<List<MachineStatusLog>> GetRecentStatusLogsAsync(int count)
        {
            return await _dbContext.MachineStatusLogs
                .Include(l => l.Machine)
                .Where(l => !l.IsDeleted && l.Machine != null && !l.Machine.IsDeleted)
                .OrderByDescending(l => l.StartTime)
                .Take(count)
                .ToListAsync();
        }

        private async Task SyncMachineStatusFromLogsAsync(int machineId)
        {
            var machine = await _dbContext.Machines.FindAsync(machineId);
            if (machine == null) return;

            var now = DateTime.Now;

            // Check if there is an active plan running on this machine right now
            var hasActivePlan = await _dbContext.ProductionPlans
                .Where(p => !p.IsDeleted && p.Status == ProductionPlanStatus.InProgress)
                .AnyAsync(p => p.PlanMachines.Any(pm => pm.Machine != null && pm.Machine.Id == machineId));

            var activeLog = await _dbContext.MachineStatusLogs
                .Where(l => !l.IsDeleted && l.Machine != null && l.Machine.Id == machineId && l.StartTime <= now && (l.EndTime == null || l.EndTime >= now))
                .OrderByDescending(l => l.StartTime)
                .FirstOrDefaultAsync();

            if (activeLog != null)
            {
                var targetStatus = activeLog.Status;

                // Override to Active if machine is currently Idle but has an active plan running
                if (targetStatus == MachineStatus.Idle && hasActivePlan)
                {
                    targetStatus = MachineStatus.Active;
                }

                machine.Status = targetStatus;
            }
            else
            {
                var latestEndedLog = await _dbContext.MachineStatusLogs
                    .Where(l => !l.IsDeleted && l.Machine != null && l.Machine.Id == machineId && l.EndTime.HasValue && l.EndTime.Value <= now)
                    .OrderByDescending(l => l.EndTime)
                    .FirstOrDefaultAsync();

                var targetStatus = latestEndedLog != null ? MachineStatus.Idle : MachineStatus.Active;

                // Override to Active if machine is currently Idle but has an active plan running
                if (targetStatus == MachineStatus.Idle && hasActivePlan)
                {
                    targetStatus = MachineStatus.Active;
                }

                if (machine.Status != targetStatus)
                {
                    var oldStatus = machine.Status;
                    machine.Status = targetStatus;

                    var systemLog = new MachineStatusLog
                    {
                        Machine = machine,
                        Status = targetStatus,
                        Description = $"Durum değişikliği: {oldStatus.GetLabel()} -> {targetStatus.GetLabel()}",
                        StartTime = now,
                        EndTime = null,
                        WeekNumber = System.Globalization.ISOWeek.GetWeekOfYear(now),
                        Year = now.Year,
                        CreatedDate = now,
                        UpdatedDate = now
                    };
                    _dbContext.MachineStatusLogs.Add(systemLog);
                }
            }

            machine.UpdatedDate = now;
            await _dbContext.SaveChangesAsync();
        }

        public async Task SyncAllMachineStatusesAsync()
        {
            var machines = await _dbContext.Machines
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            foreach (var machine in machines)
            {
                await SyncMachineStatusFromLogsAsync(machine.Id);
            }
        }
    }
}
