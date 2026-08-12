using System;
using System.Collections.Generic;
using System.Text;
using PlantNode.Domain.Entities.Products;
using PlantNode.Domain.Enums;
using PlantNode.Domain.Interfaces;

namespace PlantNode.Domain.Entities.Machines;

public class CoilWindingMachine : Machine
{
    public int MaxRpm { get; set; }
    public decimal MinWireDiameter { get; set; }
    public decimal MaxWireDiameter { get; set; }
    public decimal MaxCoilDiameter { get; set; }
    public int SpindleCount { get; set; } = 1;
    public int TargetTurnCount { get; set; }
    public int CurrentTurnCount { get; set; }
    public decimal WireTension { get; set; }
    public float TargetSpeed { get; set; }
    
    public int? CurrentProductId { get; set; }
    public Product? CurrentProduct { get; set; }

    public float CalculateOptimalSpeed(ICoilWindingSpeedStrategyFactory strategyFactory)
    {
        var material = CurrentProduct?.MaterialType;
        var strategy = strategyFactory.GetStrategy(material);
        return strategy.CalculateSpeed(this);
    }

    public void UpdateTargetSpeed(ICoilWindingSpeedStrategyFactory strategyFactory)
    {
        this.TargetSpeed = this.CalculateOptimalSpeed(strategyFactory);
    }
}
