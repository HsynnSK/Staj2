using OrderSystemManagment.Domain.Entities;

namespace OrderSystemManagment.Domain.Interfaces;

public interface IOrderRepository
{
    Task SaveOrderAsync(Order order);

    Task UpdateOrderAsync(Order order);

    Task SoftDeleteOrderAsync(int id);

    Task<IEnumerable<Order>> GetOrdersAsync();

    Task<Order?> GetOrderByIdAsync(int id);

    Task<string> GenerateInvoiceNumberAsync();

    Task<int?> GetOrderIdByOrderItemIdAsync(int orderItemId);
}