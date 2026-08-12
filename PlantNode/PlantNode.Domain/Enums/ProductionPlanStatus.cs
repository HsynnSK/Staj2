using System;
using System.Collections.Generic;
using System.Text;

namespace PlantNode.Domain.Enums
{
    public enum ProductionPlanStatus
    {
        Demand = 1,
        Scheduled = 2,
        InProgress = 3,
        Completed = 4,
        NotCompleted = 5,
        Awaiting = 6
    }
}
