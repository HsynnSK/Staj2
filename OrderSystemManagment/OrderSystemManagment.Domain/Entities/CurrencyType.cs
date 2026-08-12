using OrderSystemManagment.Domain.Entities.BaseModels;

namespace OrderSystemManagment.Domain.Entities;

public class CurrencyType : DatabaseObject
{

    public string Name { get; set; } = string.Empty;
}