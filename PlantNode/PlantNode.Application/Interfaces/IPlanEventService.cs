using System;
using System.Threading.Tasks;

namespace PlantNode.Application.Interfaces
{
    public interface IPlanEventService
    {
        event Func<int, Task>? OnPlanStatusChanged;
        void NotifyPlanStatusChanged(int planId);
    }
}
