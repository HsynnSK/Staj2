using OrderSystemManagment.Domain.Entities;
using OrderSystemManagment.Domain.Interfaces;

namespace OrderSystemManagment.DataAccess.Services;

/// <summary>
/// Application-level order service responsible for business logic:
/// invoice number generation, currency conversion and rate recording.
/// Delegates pure persistence to <see cref="IOrderRepository"/>.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly ICurrencyService _currencyService;

    public OrderService(IOrderRepository repository, ICurrencyService currencyService)
    {
        _repository = repository;
        _currencyService = currencyService;
    }

    public async Task SaveOrderAsync(Order order)
    {
        // --- Business logic ---
        var invoiceNumber = await _repository.GenerateInvoiceNumberAsync();

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceType = string.Empty,
            InvoiceDate = DateTime.Now
        };

        order.Invoice = invoice;

        var targetCurrency = order.CurrencyType?.Name ?? "TRY";
        var items = order.OrderItems
            .Select(x => (x.UnitPrice * x.Quantity, x.CurrencyType?.Name ?? "TRY"))
            .ToList();

        var (grandTotal, exchangeRate, rateDate) =
            await _currencyService.ConvertAndGetRateAsync(items, targetCurrency, invoice.InvoiceDate);

        invoice.GrandTotal = grandTotal;
        invoice.ExchangeRate = exchangeRate;
        invoice.ExchangeRateDate = rateDate;
        // ----------------------

        await _repository.SaveOrderAsync(order);
    }

    public async Task UpdateOrderAsync(Order order)
    {
        // --- Business logic ---
        var existing = await _repository.GetOrderByIdAsync(order.Id)
            ?? throw new InvalidOperationException($"Order with id {order.Id} was not found.");

        var invoiceDate = existing.Invoice?.InvoiceDate ?? DateTime.Now;

        var targetCurrency = order.CurrencyType?.Name ?? "TRY";
        var items = order.OrderItems
            .Select(x => (x.UnitPrice * x.Quantity, x.CurrencyType?.Name ?? "TRY"))
            .ToList();

        var (grandTotal, exchangeRate, rateDate) =
            await _currencyService.ConvertAndGetRateAsync(items, targetCurrency, invoiceDate);

        // Attach computed fields to the order's Invoice so the repository can persist them
        order.Invoice ??= new Invoice();
        order.Invoice.GrandTotal = grandTotal;
        order.Invoice.ExchangeRate = exchangeRate;
        order.Invoice.ExchangeRateDate = rateDate;
        // ----------------------

        await _repository.UpdateOrderAsync(order);
    }

    public Task SoftDeleteOrderAsync(int id) => _repository.SoftDeleteOrderAsync(id);

    public Task<IEnumerable<Order>> GetOrdersAsync() => _repository.GetOrdersAsync();

    public Task<Order?> GetOrderByIdAsync(int id) => _repository.GetOrderByIdAsync(id);
}
