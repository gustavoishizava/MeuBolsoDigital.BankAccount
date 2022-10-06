using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.UndoPayment;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Application.IntegrationEvents.Consumed.Transactions
{
    public class TransactionUndoPaymentIntegrationEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly TransactionUndoPaymentIntegrationEventHandler _handler;

        public TransactionUndoPaymentIntegrationEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _handler = _autoMocker.CreateInstance<TransactionUndoPaymentIntegrationEventHandler>();
        }

        [Fact]
        public async Task Handler_NotFound_DoNothing()
        {
            // Arrange
            var @event = new TransactionUndoPaymentIntegrationEvent
            {
                Id = Guid.NewGuid()
            };

            _autoMocker.GetMock<IAccountRepository>()
                       .Setup(x => x.GetTransactionByIdAsync(@event.Id))
                       .ReturnsAsync((Transaction)null);

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IAccountRepository>()
                       .Verify(x => x.GetTransactionByIdAsync(@event.Id), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handler_ReturnSuccess()
        {
            // Arrange
            var @event = new TransactionUndoPaymentIntegrationEvent
            {
                Id = Guid.NewGuid()
            };

            var transactionId = Guid.NewGuid();
            var value = 10;
            var type = TransactionType.Expense;
            var createdAt = DateTime.Now;

            var account = new Account(Guid.NewGuid(), "Test", 0, AccountType.CheckingAccount);
            account.AddTransaction(transactionId, createdAt, value, type);
            var transaction = account.GetTransaction(transactionId);

            _autoMocker.GetMock<IAccountRepository>()
                       .Setup(x => x.GetTransactionByIdAsync(@event.Id))
                       .ReturnsAsync(transaction);

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IAccountRepository>()
                       .Verify(x => x.GetTransactionByIdAsync(@event.Id), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}