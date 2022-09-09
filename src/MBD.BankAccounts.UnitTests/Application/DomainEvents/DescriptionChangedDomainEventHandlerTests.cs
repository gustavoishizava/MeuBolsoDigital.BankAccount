using System;
using System.Threading;
using MBD.BankAccounts.Application.DomainEvents;
using MBD.BankAccounts.Domain.Events;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Application.DomainEvents
{
    public class DescriptionChangedDomainEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly DescriptionChangedDomainEventHandler _handler;

        public DescriptionChangedDomainEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _handler = _autoMocker.CreateInstance<DescriptionChangedDomainEventHandler>();
        }

        [Fact]
        public async void Handler_ReturnSuccess()
        {
            // Arrange
            var @event = new DescriptionChangedDomainEvent(Guid.NewGuid(), "oldDescription", "newDescription");

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
                .Verify(x => x.CreateEventAsync<DescriptionChangedDomainEvent>(@event, "updated.description"));
        }
    }
}