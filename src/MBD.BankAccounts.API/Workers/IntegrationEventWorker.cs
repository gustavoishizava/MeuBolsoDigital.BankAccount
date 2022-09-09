using System;
using System.Threading;
using System.Threading.Tasks;
using MeuBolsoDigital.CrossCutting.Extensions;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MBD.BankAccounts.API.Workers
{
    public class IntegrationEventWorker : BackgroundService
    {
        private readonly ILogger<IntegrationEventWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public IntegrationEventWorker(ILogger<IntegrationEventWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{GetType().Name} started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessIntegrationEventsAsync();
                await Task.Delay(5000);
            }
        }

        protected async Task ProcessIntegrationEventsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var integrationEventLogService = scope.ServiceProvider.GetService<IIntegrationEventLogService>();

            var integrationEventLogs = await integrationEventLogService.RetrieveEventLogsPendingToPublishAsync();
            if (integrationEventLogs.IsNullOrEmpty())
                return;

            foreach (var integrationEventLogEntry in integrationEventLogs)
            {
                // TODO: PUBLICAR NO RABBITMQ
                await integrationEventLogService.SetEventToPublishedAsync(integrationEventLogEntry);
            }
        }
    }
}