using OrderSystemManagment.Domain.Entities;

namespace OrderSystemManagment.Domain.Interfaces;

/// <summary>
/// Application-level service for order operations.
/// Handles business logic such as currency conversion and invoice generation,
/// then delegates persistence to <see cref="IOrderRepository"/>.
/// </summary>
public interface IOrderService
{
    Task SaveOrderAsync(Order order);

    Task UpdateOrderAsync(Order order);

    Task SoftDeleteOrderAsync(int id);

    Task<IEnumerable<Order>> GetOrdersAsync();

    Task<Order?> GetOrderByIdAsync(int id);
}
