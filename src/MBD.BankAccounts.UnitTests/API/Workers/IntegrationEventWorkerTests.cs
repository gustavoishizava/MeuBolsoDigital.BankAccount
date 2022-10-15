using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.API.Workers;
using MeuBolsoDigital.IntegrationEventLog;
using MeuBolsoDigital.IntegrationEventLog.Repositories;
using MeuBolsoDigital.IntegrationEventLog.Services;
using MeuBolsoDigital.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using RabbitMQ.Client;
using Xunit;

namespace MBD.BankAccounts.UnitTests.API.Workers
{
    public class IntegrationEventWorkerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly FakeIntegrationEventWorker _worker;

        public IntegrationEventWorkerTests()
        {
            _autoMocker = new AutoMocker();

            var integrationLogService = _autoMocker.CreateInstance<IntegrationEventLogService>();
            _autoMocker.Use<IIntegrationEventLogService>(integrationLogService);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IIntegrationEventLogService>((_) => integrationLogService);

            _autoMocker.Use<IServiceProvider>(serviceCollection.BuildServiceProvider());
            _worker = _autoMocker.CreateInstance<FakeIntegrationEventWorker>();
        }

        private class FakeIntegrationEventWorker : IntegrationEventWorker
        {
            public FakeIntegrationEventWorker(ILogger<IntegrationEventWorker> logger, IServiceProvider serviceProvider, IRabbitMqConnection rabbitMqConnection, IConfiguration configuration) : base(logger, serviceProvider, rabbitMqConnection, configuration)
            {
            }

            public async Task StartProcessAsync()
            {
                await base.ProcessIntegrationEventsAsync(new CancellationToken());
            }
        }

        private class FakeEvent
        {
            public int Id { get; set; }
        }

        [Fact]
        public async Task StartProcessIntegrationEvent_HaveEvents_PublishMessages()
        {
            // Arrange
            _autoMocker.GetMock<IIntegrationEventLogRepository>()
                .SetupSequence(x => x.FindNextToPublishAsync())
                .ReturnsAsync(new IntegrationEventLogEntry("type", "content"))
                .ReturnsAsync(new IntegrationEventLogEntry("type", "content"))
                .ReturnsAsync((IntegrationEventLogEntry)null);

            _autoMocker.GetMock<IRabbitMqConnection>()
                .Setup(x => x.Channel)
                .Returns(new Mock<IModel>().Object);

            // Act
            await _worker.StartProcessAsync();

            // Assert
            _autoMocker.GetMock<IRabbitMqConnection>()
                .Verify(x => x.TryConnect(It.IsAny<CancellationToken>()), Times.Exactly(3));

            _autoMocker.GetMock<IRabbitMqConnection>()
                .Verify(x => x.Channel.ExchangeDeclare(It.IsAny<string>(),
                                                       It.IsAny<string>(),
                                                       It.IsAny<bool>(),
                                                       It.IsAny<bool>(),
                                                       It.IsAny<IDictionary<string, object>>()), Times.Once);

            _autoMocker.GetMock<IRabbitMqConnection>()
                .Verify(x => x.Channel.BasicPublish(It.IsAny<string>(),
                                                    It.IsAny<string>(),
                                                    It.IsAny<bool>(),
                                                    It.IsAny<IBasicProperties>(),
                                                    It.IsAny<ReadOnlyMemory<byte>>()), Times.Exactly(2));
        }
    }
}