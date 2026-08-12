using System.Threading.Tasks;

namespace OrderSystemManagment.Domain.Interfaces;

public interface IFindOrderFromOrderItemService
{
    Task<int?> GetOrderIdByOrderItemIdAsync(int orderItemId);
}
