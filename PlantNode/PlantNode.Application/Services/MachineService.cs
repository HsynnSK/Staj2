using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantNode.Application.Interfaces;
using PlantNode.Domain.Entities.Machines;

namespace PlantNode.Application.Services
{
    public class MachineService : IMachineService
    {
        private readonly IPlantNodeDbContext _dbContext;

        public MachineService(IPlantNodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Machine>> GetAllActiveMachinesAsync()
        {
            return await _dbContext.Machines
                .Include(m => m.Plant)
                .Include("CurrentProduct.MaterialType")
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<Machine?> GetMachineByIdAsync(int id)
        {
            return await _dbContext.Machines
                .Include(m => m.Plant)
                .Include("CurrentProduct.MaterialType")
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task SaveMachineAsync(Machine machine)
        {
            if (machine.Id == 0)
            {
                if (machine.Plant != null)
                {
                    var trackedPlant = await _dbContext.Plants.FindAsync(machine.Plant.Id);
                    if (trackedPlant != null)
                    {
                        machine.Plant = trackedPlant;
                    }
                }

                if (machine is CoilWindingMachine cwm)
                {
                    if (cwm.CurrentProductId.HasValue)
                    {
                        var trackedProduct = await _dbContext.Products.FindAsync(cwm.CurrentProductId.Value);
                        if (trackedProduct != null)
                        {
                            cwm.CurrentProduct = trackedProduct;
                        }
                    }
                    else
                    {
                        cwm.CurrentProduct = null;
                    }
                }

                _dbContext.Machines.Add(machine);
            }
            else
            {
                var existingMachine = await _dbContext.Machines
                    .Include(m => m.Plant)
                    .Include("CurrentProduct.MaterialType")
                    .FirstOrDefaultAsync(m => m.Id == machine.Id);

                if (existingMachine != null)
                {
                    existingMachine.Name = machine.Name;
                    existingMachine.MachineCode = machine.MachineCode;
                    existingMachine.SerialNumber = machine.SerialNumber;
                    existingMachine.Width = machine.Width;
                    existingMachine.Height = machine.Height;
                    existingMachine.Depth = machine.Depth;
                    existingMachine.Status = machine.Status;
                    existingMachine.UpdatedDate = DateTime.Now;

                    if (machine.Plant != null)
                    {
                        existingMachine.Plant = await _dbContext.Plants.FindAsync(machine.Plant.Id);
                    }
                    else
                    {
                        existingMachine.Plant = null;
                    }

                    if (existingMachine is CoilWindingMachine existingCwm && machine is CoilWindingMachine incomingCwm)
                    {
                        existingCwm.MaxRpm = incomingCwm.MaxRpm;
                        existingCwm.SpindleCount = incomingCwm.SpindleCount;
                        existingCwm.MinWireDiameter = incomingCwm.MinWireDiameter;
                        existingCwm.MaxWireDiameter = incomingCwm.MaxWireDiameter;
                        existingCwm.MaxCoilDiameter = incomingCwm.MaxCoilDiameter;
                        existingCwm.TargetSpeed = incomingCwm.TargetSpeed;
                        existingCwm.CurrentProductId = incomingCwm.CurrentProductId;

                        if (incomingCwm.CurrentProductId.HasValue)
                        {
                            existingCwm.CurrentProduct = await _dbContext.Products.FindAsync(incomingCwm.CurrentProductId.Value);
                        }
                        else
                        {
                            existingCwm.CurrentProduct = null;
                        }
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMachineAsync(int id)
        {
            var machine = await _dbContext.Machines.FindAsync(id);
            if (machine != null)
            {
                machine.IsDeleted = true;
                machine.UpdatedDate = System.DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
