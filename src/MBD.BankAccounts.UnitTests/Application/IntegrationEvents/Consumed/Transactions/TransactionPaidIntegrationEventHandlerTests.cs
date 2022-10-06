using System;
using System.Threading;
using MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.Paid;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Application.IntegrationEvents.Consumed.Transactions
{
    public class TransactionPaidIntegrationEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly TransactionPaidIntegrationEventHandler _handler;

        public TransactionPaidIntegrationEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _handler = _autoMocker.CreateInstance<TransactionPaidIntegrationEventHandler>();
        }

        [Fact]
        public async void Handle_ReturnSuccess()
        {
            // Arrange
            var @event = new TransactionPaidIntegrationEvent
            {
                Id = Guid.NewGuid(),
                BankAccountId = Guid.NewGuid(),
                Date = DateTime.Now,
                Value = 100,
                Type = "Expense"
            };

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<ICreateOrUpdateTransactionService>().Verify(x => x.CreateOrUpdateAsync(@event.BankAccountId,
                                                                                                       @event.Id,
                                                                                                       @event.Value,
                                                                                                       Enum.Parse<TransactionType>(@event.Type),
                                                                                                       @event.Date), Times.Once);
        }
    }
}