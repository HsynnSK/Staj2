using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderSystemManagment.DataAccess.Context;
using OrderSystemManagment.Domain.Interfaces;

namespace OrderSystemManagment.web.Services;

/// <summary>
/// Background service that runs once at startup to retroactively correct
/// any orders whose GrandTotal, ExchangeRate or ExchangeRateDate is outdated
/// or was never computed (e.g. created before the TCMB integration existed).
/// </summary>
public class RetroactiveOrderFixerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RetroactiveOrderFixerService> _logger;

    public RetroactiveOrderFixerService(
        IServiceProvider serviceProvider,
        ILogger<RetroactiveOrderFixerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            var currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();

            var ordersToFix = await dbContext.Orders
                .Include(o => o.Invoice)
                .Include(o => o.CurrencyType)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.CurrencyType)
                .Where(o => o.Invoice != null)
                .ToListAsync(stoppingToken);

            if (!ordersToFix.Any())
                return;

            foreach (var order in ordersToFix)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                var invoice = order.Invoice!;
                var targetCurrency = order.CurrencyType?.Name ?? "TRY";
                var items = order.OrderItems
                    .Select(x => (x.UnitPrice * x.Quantity, x.CurrencyType?.Name ?? "TRY"))
                    .ToList();

                try
                {
                    var (grandTotal, exchangeRate, rateDate) =
                        await currencyService.ConvertAndGetRateAsync(items, targetCurrency, invoice.InvoiceDate);

                    if (Math.Abs(invoice.GrandTotal - grandTotal) > 0.0001 ||
                        invoice.ExchangeRate != exchangeRate ||
                        invoice.ExchangeRateDate != rateDate)
                    {
                        invoice.GrandTotal = grandTotal;
                        invoice.ExchangeRate = exchangeRate;
                        invoice.ExchangeRateDate = rateDate;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Could not fix order {OrderId} using invoice date {InvoiceDate}. Retrying with today's rates.",
                        order.Id, invoice.InvoiceDate);

                    try
                    {
                        var (grandTotal, exchangeRate, rateDate) =
                            await currencyService.ConvertAndGetRateAsync(items, targetCurrency, DateTime.Today);

                        invoice.GrandTotal = grandTotal;
                        invoice.ExchangeRate = exchangeRate;
                        invoice.ExchangeRateDate = rateDate;
                    }
                    catch (Exception fallbackEx)
                    {
                        _logger.LogError(fallbackEx,
                            "Skipping order {OrderId}: TCMB unavailable.", order.Id);
                    }
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("RetroactiveOrderFixerService completed for {Count} orders.", ordersToFix.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RetroactiveOrderFixerService failed during startup fix.");
        }
    }
}
