using PlantNode.Domain.Interfaces;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Entities.Products;
using System;

namespace PlantNode.Domain.Strategies;

public class MaterialTypeSpeedStrategy : ICoilWindingSpeedStrategy
{
    public float CalculateSpeed(CoilWindingMachine machine)
    {
        var diameter = machine.MinWireDiameter;
        float baseSpeed = 0f;
        if (diameter >= 0.1m && diameter <= 1.7m)
            baseSpeed = 25f;
        else if (diameter >= 1.8m && diameter <= 2.1m)
            baseSpeed = 0.22f;
        else if (diameter >= 2.2m && diameter <= 3.2m)
            baseSpeed = 0.16f;

        float spindleMultiplier = 1.0f;
        if (machine.SpindleCount >= 3 && machine.SpindleCount <= 4)
            spindleMultiplier = 0.9f;
        else if (machine.SpindleCount > 4)
            spindleMultiplier = 0.8f;

        float materialMultiplier = 1.0f;
        float tensionMultiplier = 1.0f;

        var material = machine.CurrentProduct?.MaterialType;
        if (material != null)
        {
            materialMultiplier = material.MaxMachineSpeed;

            if (machine.WireTension > material.TensionLimit)
            {
                tensionMultiplier = 0.85f;
            }
            else if (machine.WireTension < 10)
            {
                tensionMultiplier = 1.1f;
            }
        }
        else
        {
            if (machine.WireTension < 10)
                tensionMultiplier = 1.1f;
            else if (machine.WireTension > 20)
                tensionMultiplier = 0.85f;
        }

        return baseSpeed * spindleMultiplier * tensionMultiplier * materialMultiplier;
    }
}
