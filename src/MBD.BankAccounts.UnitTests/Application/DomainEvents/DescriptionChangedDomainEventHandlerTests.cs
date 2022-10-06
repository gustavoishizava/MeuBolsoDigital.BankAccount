using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Application.DomainEvents;
using MBD.BankAccounts.Application.IntegrationEvents.Produced.BankAccounts.DescriptionChanged;
using MBD.BankAccounts.Domain.Events;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq;
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
        public async Task Handler_ReturnSuccess()
        {
            // Arrange
            var @event = new DescriptionChangedDomainEvent(Guid.NewGuid(), "oldDescription", "newDescription");

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
                .Verify(x => x.CreateEventAsync<BankAccountDescriptionChangedIntegrationEvent>(It.Is<BankAccountDescriptionChangedIntegrationEvent>(x => x.Id == @event.Id
                                                                                                                                                         && x.OldDescription == @event.OldDescription
                                                                                                                                                         && x.NewDescription == @event.NewDescription), "updated.description"));
        }
    }
}