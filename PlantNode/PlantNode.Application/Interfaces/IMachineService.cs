using System.Collections.Generic;
using System.Threading.Tasks;
using PlantNode.Domain.Entities.Machines;

namespace PlantNode.Application.Interfaces;

public interface IMachineService
{
    Task<List<Machine>> GetAllActiveMachinesAsync();
    Task<Machine?> GetMachineByIdAsync(int id);
    Task SaveMachineAsync(Machine machine);
    Task DeleteMachineAsync(int id);
}
