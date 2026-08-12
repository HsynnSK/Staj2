using System;
using System.Collections.Generic;
using System.Text;

namespace PlantNode.Domain.Entities.Plants;

public class CoilWindingPlant : Plant
{
    public decimal MaxSupportedWireGauge { get; set; }
    public int MaxAnnealingTemperatureCelsius { get; set; }
}
