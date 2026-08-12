using System;
using System.Threading.Tasks;
using PlantNode.Application.Interfaces;

namespace PlantNode.Application.Services
{
    public class PlanEventService : IPlanEventService
    {
        public event Func<int, Task>? OnPlanStatusChanged;

        public void NotifyPlanStatusChanged(int planId)
        {
            var handlers = OnPlanStatusChanged?.GetInvocationList();
            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await ((Func<int, Task>)handler)(planId);
                        }
                        catch
                        {
                            // Catch and ignore handler exceptions to prevent blocking other subscribers
                        }
                    });
                }
            }
        }
    }
}
