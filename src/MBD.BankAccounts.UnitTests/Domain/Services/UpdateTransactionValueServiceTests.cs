using System;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Domain.Interfaces.Services;
using MBD.BankAccounts.Domain.Services;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Domain.Services
{
    public class UpdateTransactionValueServiceTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly IUpdateTransactionValueService _service;

        public UpdateTransactionValueServiceTests()
        {
            _autoMocker = new AutoMocker();
            _service = _autoMocker.CreateInstance<UpdateTransactionValueService>();
        }

        [Fact]
        public async Task UpdateValue_NotFound_DoNothing()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var value = 10;

            _autoMocker.GetMock<IAccountRepository>()
                       .Setup(x => x.GetTransactionByIdAsync(transactionId))
                       .ReturnsAsync((Transaction)null);

            // Act
            var result = await _service.UpdateValueAsync(transactionId, value);

            // Assert
            Assert.False(result);

            _autoMocker.GetMock<IAccountRepository>()
                       .Verify(x => x.GetTransactionByIdAsync(transactionId), Times.Once);

            _autoMocker.GetMock<IAccountRepository>()
                       .Verify(x => x.UpdateTransactionAsync(It.IsAny<Transaction>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateValue_ReturnSuccess()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var value = 10;
            var oldValue = 5;
            var type = TransactionType.Expense;
            var createdAt = DateTime.Now;

            var account = new Account(Guid.NewGuid(), "Test", 0, AccountType.CheckingAccount);
            account.AddTransaction(transactionId, createdAt, oldValue, type);
            var transaction = account.GetTransaction(transactionId);

            _autoMocker.GetMock<IAccountRepository>()
                       .Setup(x => x.GetTransactionByIdAsync(transactionId))
                       .ReturnsAsync(transaction);

            // Act
            var result = await _service.UpdateValueAsync(transactionId, value);

            // Assert
            Assert.False(result);

            _autoMocker.GetMock<IAccountRepository>()
                       .Verify(x => x.GetTransactionByIdAsync(transactionId), Times.Once);

            _autoMocker.GetMock<IAccountRepository>()
                       .Verify(x => x.UpdateTransactionAsync(It.Is<Transaction>(x => x.Value.Value == value)), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}