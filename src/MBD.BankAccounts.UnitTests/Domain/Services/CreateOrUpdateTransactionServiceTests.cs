using System;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Domain.Interfaces.Services;
using MBD.BankAccounts.Domain.Services;
using MeuBolsoDigital.Core.Exceptions;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Domain.Services
{
    public class CreateOrUpdateTransactionServiceTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly ICreateOrUpdateTransactionService _service;

        public CreateOrUpdateTransactionServiceTests()
        {
            _autoMocker = new AutoMocker();
            _autoMocker.GetMock<IUnitOfWork>().Setup(x => x.CommitAsync()).ReturnsAsync(true);
            _service = _autoMocker.CreateInstance<CreateOrUpdateTransactionService>();
        }

        [Fact]
        public async Task AccountNotFound_DoNothing()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var transactionId = Guid.NewGuid();
            var value = 10;
            var type = TransactionType.Expense;
            var createdAt = DateTime.Now;

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();

            repositoryMock.Setup(x => x.GetByIdWithoutUserAsync(accountId))
                          .ReturnsAsync((Account)null);

            // Act
            var result = await _service.CreateOrUpdateAsync(accountId, transactionId, value, type, createdAt);

            // Assert
            Assert.True(result);

            repositoryMock.Verify(x => x.GetByIdWithoutUserAsync(accountId), Times.Once);
            repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Account>()), Times.Never);
            repositoryMock.Verify(x => x.RemoveTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task TransactionNotExists_AddTransaction()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var transactionId = Guid.NewGuid();
            var value = 10;
            var type = TransactionType.Expense;
            var createdAt = DateTime.Now;
            var account = new Account(Guid.NewGuid(), "Test", 0, AccountType.CheckingAccount);

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();

            repositoryMock.Setup(x => x.GetByIdWithoutUserAsync(accountId))
                          .ReturnsAsync(account);

            repositoryMock.Setup(x => x.GetTransactionByIdAsync(transactionId))
                          .ReturnsAsync((Transaction)null);

            // Act
            var result = await _service.CreateOrUpdateAsync(accountId, transactionId, value, type, createdAt);

            // Assert
            Assert.True(result);

            repositoryMock.Verify(x => x.GetByIdWithoutUserAsync(accountId), Times.Once);
            repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Account>()), Times.Once);
            repositoryMock.Verify(x => x.RemoveTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task TransactionExists_AccountChanged_OldAccountNotFound_ReturnDomainException()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var value = 10;
            var type = TransactionType.Expense;
            var createdAt = DateTime.Now;

            var oldAccount = new Account(Guid.NewGuid(), "Test", 0, AccountType.CheckingAccount);
            oldAccount.AddTransaction(transactionId, createdAt, value, type);
            var transaction = oldAccount.GetTransaction(transactionId);

            var newAccount = new Account(Guid.NewGuid(), "Test", 0, AccountType.CheckingAccount);

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();

            repositoryMock.Setup(x => x.GetByIdWithoutUserAsync(newAccount.Id))
                          .ReturnsAsync(newAccount);

            repositoryMock.Setup(x => x.GetByIdWithoutUserAsync(transaction.AccountId))
                          .ReturnsAsync((Account)null);

            repositoryMock.Setup(x => x.GetTransactionByIdAsync(transactionId))
                          .ReturnsAsync(transaction);

            // Act && Assert
            await Assert.ThrowsAsync<DomainException>(() => _service.CreateOrUpdateAsync(newAccount.Id, transactionId, value, type, createdAt));

            repositoryMock.Verify(x => x.GetByIdWithoutUserAsync(newAccount.Id), Times.Once);
            repositoryMock.Verify(x => x.GetByIdWithoutUserAsync(transaction.AccountId), Times.Once);
            repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Account>()), Times.Never);
            repositoryMock.Verify(x => x.RemoveTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task TransactionExists_AccountChanged_RemoveOldTransaction_AddNewTransaction()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var value = 10;
            var type = TransactionType.Expense;
            var createdAt = DateTime.Now;

            var oldAccount = new Account(Guid.NewGuid(), "Test", 0, AccountType.CheckingAccount);
            oldAccount.AddTransaction(transactionId, createdAt, value, type);
            var transaction = oldAccount.GetTransaction(transactionId);

            var newAccount = new Account(Guid.NewGuid(), "Test", 0, AccountType.CheckingAccount);

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();

            repositoryMock.Setup(x => x.GetByIdWithoutUserAsync(newAccount.Id))
                          .ReturnsAsync(newAccount);

            repositoryMock.Setup(x => x.GetByIdWithoutUserAsync(transaction.AccountId))
                          .ReturnsAsync(oldAccount);

            repositoryMock.Setup(x => x.GetTransactionByIdAsync(transactionId))
                          .ReturnsAsync(transaction);

            // Act
            var result = await _service.CreateOrUpdateAsync(newAccount.Id, transactionId, value, type, createdAt);

            // Assert
            Assert.True(result);

            repositoryMock.Verify(x => x.GetByIdWithoutUserAsync(newAccount.Id), Times.Once);
            repositoryMock.Verify(x => x.GetByIdWithoutUserAsync(transaction.AccountId), Times.Once);
            repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Account>()), Times.Exactly(2));
            repositoryMock.Verify(x => x.RemoveTransactionAsync(It.IsAny<Transaction>()), Times.Once);
            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransactionExists_AccountNotChanged_AddOrUpdateTransaction(bool transactionExists)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var value = 10;
            var type = TransactionType.Expense;
            var createdAt = DateTime.Now;

            var account = new Account(Guid.NewGuid(), "Test", 0, AccountType.CheckingAccount);
            account.AddTransaction(transactionId, createdAt, value, type);
            var transaction = account.GetTransaction(transactionId);

            if (!transactionExists)
                account.RemoveTransaction(transactionId);

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();

            repositoryMock.Setup(x => x.GetByIdWithoutUserAsync(account.Id))
                          .ReturnsAsync(account);

            repositoryMock.Setup(x => x.GetTransactionByIdAsync(transactionId))
                          .ReturnsAsync(transaction);

            // Act
            var result = await _service.CreateOrUpdateAsync(account.Id, transactionId, value, type, createdAt);

            // Assert
            Assert.True(result);

            repositoryMock.Verify(x => x.GetByIdWithoutUserAsync(account.Id), Times.Once);
            repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Account>()), Times.Once);
            repositoryMock.Verify(x => x.RemoveTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            _autoMocker.GetMock<IUnitOfWork>().Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}