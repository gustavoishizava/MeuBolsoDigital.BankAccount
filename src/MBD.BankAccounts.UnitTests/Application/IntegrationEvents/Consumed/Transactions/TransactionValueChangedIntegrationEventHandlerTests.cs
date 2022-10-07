using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.ValueChanged;
using MBD.BankAccounts.Domain.Interfaces.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Application.IntegrationEvents.Consumed.Transactions
{
    public class TransactionValueChangedIntegrationEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly TransactionValueChangedIntegrationEventHandler _handler;

        public TransactionValueChangedIntegrationEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _handler = _autoMocker.CreateInstance<TransactionValueChangedIntegrationEventHandler>();
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var @event = new TransactionValueChangedIntegrationEvent
            {
                Id = Guid.NewGuid(),
                NewValue = 100
            };

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IUpdateTransactionValueService>().Verify(x => x.UpdateValueAsync(@event.Id, @event.NewValue), Times.Once);
        }
    }
}