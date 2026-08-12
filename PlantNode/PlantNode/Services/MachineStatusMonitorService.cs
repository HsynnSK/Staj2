using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlantNode.Application.Interfaces;

namespace PlantNode.Services;

public class MachineStatusMonitorService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<MachineStatusMonitorService> _logger;

    public MachineStatusMonitorService(IServiceScopeFactory serviceScopeFactory, ILogger<MachineStatusMonitorService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Machine Status Monitor Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var maintenanceService = scope.ServiceProvider.GetRequiredService<IMaintenanceService>();
                    
                    await maintenanceService.SyncAllMachineStatusesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during machine status monitoring background task run.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        _logger.LogInformation("Machine Status Monitor Service stopped.");
    }
}
