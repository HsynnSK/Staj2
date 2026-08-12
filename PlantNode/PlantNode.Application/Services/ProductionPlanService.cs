using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantNode.Application.Interfaces;
using PlantNode.Domain.Entities.Production;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Enums;
using PlantNode.Domain.Interfaces;

namespace PlantNode.Application.Services
{
    public class ProductionPlanService : IProductionPlanService
    {
        private readonly IPlantNodeDbContext _dbContext;
        private readonly ICoilWindingSpeedStrategyFactory _speedStrategyFactory;

        public ProductionPlanService(IPlantNodeDbContext dbContext, ICoilWindingSpeedStrategyFactory speedStrategyFactory)
        {
            _dbContext = dbContext;
            _speedStrategyFactory = speedStrategyFactory;
        }

        public async Task<List<ProductionPlan>> GetAllProductionPlansAsync()
        {
            return await _dbContext.ProductionPlans
                .AsNoTracking()
                .Include(p => p.PlanProducts)
                    .ThenInclude(pp => pp.Product)
                .Include(p => p.PlanMachines)
                    .ThenInclude(pm => pm.Machine)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.PlannedStartDate)
                .ToListAsync();
        }

        public async Task<ProductionPlan?> GetProductionPlanByIdAsync(int id)
        {
            return await _dbContext.ProductionPlans
                .AsNoTracking()
                .Include(p => p.PlanProducts)
                    .ThenInclude(pp => pp.Product)
                .Include(p => p.PlanMachines)
                    .ThenInclude(pm => pm.Machine)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        private async Task CalculateEstimationAsync(ProductionPlan plan)
        {
            if (plan.Status == ProductionPlanStatus.Demand)
            {
                return;
            }

            double totalHourlyCapacity = 0;

            foreach (var pm in plan.PlanMachines)
            {
                if (pm.Machine == null) continue;

                var machine = await _dbContext.Machines
                    .Include("CurrentProduct.MaterialType")
                    .FirstOrDefaultAsync(m => m.Id == pm.Machine.Id);

                if (machine == null) continue;

                bool isUnavailable = machine.Status == MachineStatus.Maintenance || 
                                     machine.Status == MachineStatus.Malfunction;
                if (isUnavailable) continue;

                double capacity = 0;
                if (machine is CoilWindingMachine cwm)
                {
                    if (plan.PlanProducts.Any())
                    {
                        var firstProduct = plan.PlanProducts.First().Product;
                        if (firstProduct != null)
                        {
                            cwm.CurrentProduct = await _dbContext.Products
                                .Include(p => p.MaterialType)
                                .FirstOrDefaultAsync(p => p.Id == firstProduct.Id);
                        }
                    }

                    var speed = cwm.CalculateOptimalSpeed(_speedStrategyFactory);
                    capacity = speed > 0 ? speed * 60 : 50; 
                }
                else
                {
                    capacity = 30; 
                }

                totalHourlyCapacity += capacity;
            }

            if (totalHourlyCapacity <= 0)
            {
                totalHourlyCapacity = 50; 
            }

            double hoursNeeded = (double)plan.TargetQuantity / totalHourlyCapacity;
            
            bool isAutomatic = plan.Description != null && 
                              (plan.Description.Contains("Otomatik") || 
                               plan.Description.Contains("akıllı") || 
                               plan.Description.Contains("Auto"));

            if (!isAutomatic)
            {
                // For manual plans, limit target quantity and duration to exactly what can be produced in this 1-hour slot
                if (plan.TargetQuantity > totalHourlyCapacity)
                {
                    plan.TargetQuantity = (int)totalHourlyCapacity;
                }
                
                hoursNeeded = (double)plan.TargetQuantity / totalHourlyCapacity;
                plan.EstimatedEndDate = plan.PlannedStartDate.AddHours(Math.Min(1.0, hoursNeeded));
                plan.PlannedEndDate = plan.EstimatedEndDate;
            }
            else
            {
                plan.EstimatedEndDate = plan.PlannedStartDate.AddHours(hoursNeeded);
                plan.PlannedEndDate = plan.EstimatedEndDate;
            }
        }

        public async Task SaveProductionPlanAsync(ProductionPlan plan)
        {
            if (plan.Status == ProductionPlanStatus.Demand)
            {
                plan.PlannedStartDate = DateTime.MinValue;
                plan.PlannedEndDate = DateTime.MinValue;
                plan.PlanMachines.Clear();
            }
            else
            {
                if (plan.Status != ProductionPlanStatus.Completed &&
                    plan.Status != ProductionPlanStatus.NotCompleted &&
                    plan.Status != ProductionPlanStatus.Awaiting)
                {
                    await CalculateEstimationAsync(plan);

                    var now = DateTime.Now;
                    if (plan.PlannedStartDate > now)
                    {
                        plan.Status = ProductionPlanStatus.Scheduled;
                    }
                    else if (plan.PlannedStartDate <= now && plan.PlannedEndDate >= now)
                    {
                        plan.Status = ProductionPlanStatus.InProgress;
                    }
                    else
                    {
                        plan.Status = ProductionPlanStatus.Awaiting;
                    }
                }
            }

            if (plan.Id == 0)
            {
                foreach (var pp in plan.PlanProducts.ToList())
                {
                    if (pp.Product != null)
                    {
                        var trackedProduct = await _dbContext.Products.FindAsync(pp.Product.Id);
                        if (trackedProduct != null)
                        {
                            pp.Product = trackedProduct;
                        }
                    }
                }
                foreach (var pm in plan.PlanMachines.ToList())
                {
                    if (pm.Machine != null)
                    {
                        var trackedMachine = await _dbContext.Machines.FindAsync(pm.Machine.Id);
                        if (trackedMachine != null)
                        {
                            pm.Machine = trackedMachine;
                        }
                    }
                }
                _dbContext.ProductionPlans.Add(plan);
            }
            else
            {
                var existingPlan = await _dbContext.ProductionPlans
                    .Include(p => p.PlanProducts)
                    .Include(p => p.PlanMachines)
                    .FirstOrDefaultAsync(p => p.Id == plan.Id);

                if (existingPlan != null)
                {
                    _dbContext.ProductionPlanProducts.RemoveRange(existingPlan.PlanProducts);
                    _dbContext.ProductionPlanMachines.RemoveRange(existingPlan.PlanMachines);

                    existingPlan.PlanCode = plan.PlanCode;
                    existingPlan.Description = plan.Description;
                    existingPlan.PlannedStartDate = plan.PlannedStartDate;
                    existingPlan.PlannedEndDate = plan.PlannedEndDate;
                    existingPlan.Status = plan.Status;
                    existingPlan.TargetQuantity = plan.TargetQuantity;
                    existingPlan.EstimatedEndDate = plan.EstimatedEndDate;
                    existingPlan.UpdatedDate = DateTime.Now;

                    foreach (var pp in plan.PlanProducts)
                    {
                        if (pp.Product != null)
                        {
                            var trackedProduct = await _dbContext.Products.FindAsync(pp.Product.Id);
                            if (trackedProduct != null)
                            {
                                pp.Product = trackedProduct;
                            }
                        }
                        existingPlan.PlanProducts.Add(pp);
                    }
                    foreach (var pm in plan.PlanMachines)
                    {
                        if (pm.Machine != null)
                        {
                            var trackedMachine = await _dbContext.Machines.FindAsync(pm.Machine.Id);
                            if (trackedMachine != null)
                            {
                                pm.Machine = trackedMachine;
                            }
                        }
                        existingPlan.PlanMachines.Add(pm);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            await SyncAllPlanStatusesAsync();
        }

        public async Task DeleteProductionPlanAsync(int id)
        {
            var plan = await _dbContext.ProductionPlans
                .Include(p => p.PlanProducts)
                .Include(p => p.PlanMachines)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan != null)
            {
                _dbContext.ProductionPlanProducts.RemoveRange(plan.PlanProducts);
                _dbContext.ProductionPlanMachines.RemoveRange(plan.PlanMachines);

                plan.IsDeleted = true;
                plan.UpdatedDate = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task SyncAllPlanStatusesAsync()
        {
            var plans = await _dbContext.ProductionPlans
                .Include(p => p.PlanMachines)
                    .ThenInclude(pm => pm.Machine)
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            var now = DateTime.Now;

            foreach (var plan in plans)
            {
                if (plan.Status == ProductionPlanStatus.Demand) continue;

                // If status is manually set or completed/notcompleted/awaiting, preserve it and only free machines if needed
                if (plan.Status == ProductionPlanStatus.Completed || 
                    plan.Status == ProductionPlanStatus.NotCompleted || 
                    plan.Status == ProductionPlanStatus.Awaiting)
                {
                    foreach (var pm in plan.PlanMachines)
                    {
                        var machine = pm.Machine;
                        if (machine != null && machine.Status == MachineStatus.Active)
                        {
                            var activeLog = await _dbContext.MachineStatusLogs
                                .Where(l => !l.IsDeleted && l.Machine != null && l.Machine.Id == machine.Id && l.EndTime == null && l.Status == MachineStatus.Active && l.Description != null && l.Description.Contains(plan.PlanCode))
                                .OrderByDescending(l => l.StartTime)
                                .FirstOrDefaultAsync();

                            if (activeLog != null)
                            {
                                activeLog.EndTime = now;

                                machine.Status = MachineStatus.Idle;
                                _dbContext.MachineStatusLogs.Add(new MachineStatusLog
                                {
                                    Machine = machine,
                                    Status = MachineStatus.Idle,
                                    Description = $"Plan sonlandı/beklemede ({plan.PlanCode}): Aktif -> Boşta",
                                    StartTime = now,
                                    WeekNumber = System.Globalization.ISOWeek.GetWeekOfYear(now),
                                    Year = now.Year,
                                    CreatedDate = now,
                                    UpdatedDate = now
                                });
                            }
                        }
                    }
                    continue;
                }

                await CalculateEstimationAsync(plan);

                if (plan.PlannedStartDate > now)
                {
                    plan.Status = ProductionPlanStatus.Scheduled;
                }
                else if (plan.PlannedStartDate <= now && plan.PlannedEndDate >= now)
                {
                    plan.Status = ProductionPlanStatus.InProgress;
                }
                else
                {
                    // Transition to Awaiting (Bekleniyor) state when duration ends
                    plan.Status = ProductionPlanStatus.Awaiting;
                }

                if (plan.Status == ProductionPlanStatus.InProgress)
                {
                    foreach (var pm in plan.PlanMachines)
                    {
                        var machine = pm.Machine;
                        if (machine != null && machine.Status == MachineStatus.Idle)
                        {
                            machine.Status = MachineStatus.Active;
                            _dbContext.MachineStatusLogs.Add(new MachineStatusLog
                            {
                                Machine = machine,
                                Status = MachineStatus.Active,
                                Description = $"Plan başladı ({plan.PlanCode}): Boşta -> Aktif",
                                StartTime = now,
                                WeekNumber = System.Globalization.ISOWeek.GetWeekOfYear(now),
                                Year = now.Year,
                                CreatedDate = now,
                                UpdatedDate = now
                            });
                        }
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
