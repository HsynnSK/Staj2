using OrderSystemManagment.Domain.Interfaces;
using System.Threading.Tasks;

namespace OrderSystemManagment.DataAccess.Services;

public class FindOrderFromOrderItemService : IFindOrderFromOrderItemService
{
    private readonly IOrderRepository _repository;

    public FindOrderFromOrderItemService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<int?> GetOrderIdByOrderItemIdAsync(int orderItemId)
    {
        return await _repository.GetOrderIdByOrderItemIdAsync(orderItemId);
    }
}