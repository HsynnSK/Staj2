using OrderSystemManagment.Domain.Entities.BaseModels;

namespace OrderSystemManagment.Domain.Entities;

public class Customer : DatabaseObject
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}