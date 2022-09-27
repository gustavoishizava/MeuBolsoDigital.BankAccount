using System;
using System.Threading.Tasks;
using Bogus;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Domain.Services;
using MeuBolsoDigital.Core.Exceptions;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Domain.Services
{
    public class TransactionManagementServiceTests
    {
        private readonly AutoMocker _mock;
        private readonly Faker _faker;
        private readonly TransactionManagementService _service;

        public TransactionManagementServiceTests()
        {
            _mock = new AutoMocker();
            _faker = new Faker();

            _service = _mock.CreateInstance<TransactionManagementService>();
        }

        [Theory(DisplayName = "Adicionar uma nova transação a uma conta existente deve retornar sucesso.")]
        [InlineData(TransactionType.Expense)]
        [InlineData(TransactionType.Income)]
        public async Task AccountExists_AddTransaction_ReturnSuccess(TransactionType transactionType)
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), _faker.Finance.AccountName(), _faker.Finance.Amount(100, 1000), AccountType.CheckingAccount);
            var transactionId = Guid.NewGuid();
            var value = _faker.Finance.Amount(10, 50);
            var createdAt = DateTime.Now.AddDays(_faker.Random.Int(1, 10000));
            Transaction transaction;

            _mock.GetMock<IAccountRepository>()
                .Setup(method => method.GetByIdWithoutUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(account);

            // Act
            await _service.AddTransactionToAccountAsync(
                Guid.NewGuid(),
                transactionId,
                value,
                transactionType,
                createdAt);

            transaction = account.GetTransaction(transactionId);

            // Assert
            Assert.NotNull(transaction);

            _mock.GetMock<IAccountRepository>()
                .Verify(method => method.GetByIdWithoutUserAsync(It.IsAny<Guid>()), Times.Once);

            _mock.GetMock<IUnitOfWork>()
                .Verify(method => method.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Ao adicionar uma transação, caso a conta não seja encontrada, deve retornar uma DomainException.")]
        public async Task AccountNotFound_AddTransaction_ReturnDomainException()
        {
            // Arrange
            _mock.GetMock<IAccountRepository>()
                .Setup(method => method.GetByIdWithoutUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Account)null);

            // Act && Assert
            await Assert.ThrowsAsync<DomainException>(async () =>
                await _service.AddTransactionToAccountAsync(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    _faker.Finance.Amount(1, 50),
                    TransactionType.Income,
                    DateTime.Now));

            _mock.GetMock<IAccountRepository>()
                .Verify(method => method.GetByIdWithoutUserAsync(It.IsAny<Guid>()), Times.Once);

            _mock.GetMock<IUnitOfWork>()
                .Verify(method => method.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "Remover uma transação existente deve retornar sucesso.")]
        public async Task ExistsTransaction_RemoveTransaction_ReturnSuccess()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), "account", 1000, AccountType.CheckingAccount);
            var transactionId = Guid.NewGuid();
            account.AddTransaction(transactionId, DateTime.Now, 100, TransactionType.Income);
            var transaction = account.GetTransaction(transactionId);

            _mock.GetMock<IAccountRepository>()
                .Setup(method => method.GetTransactionByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(transaction);

            // Act
            await _service.RemoveTransactionAsync(Guid.NewGuid());

            // Assert
            _mock.GetMock<IAccountRepository>()
                .Verify(method => method.GetTransactionByIdAsync(It.IsAny<Guid>()), Times.Once);

            _mock.GetMock<IAccountRepository>()
                .Verify(method => method.RemoveTransactionAsync(transaction), Times.Once);

            _mock.GetMock<IUnitOfWork>()
                .Verify(method => method.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Remover uma transação inexistente não deve causar erros nem efeitos colaterais.")]
        public async Task NotFoundTransaction_RemoveTransaction_DoNothing()
        {
            // Arrange
            _mock.GetMock<IAccountRepository>()
                .Setup(method => method.GetTransactionByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Transaction)null);

            // Act
            await _service.RemoveTransactionAsync(Guid.NewGuid());

            // Assert
            _mock.GetMock<IAccountRepository>()
                .Verify(method => method.GetTransactionByIdAsync(It.IsAny<Guid>()), Times.Once);

            _mock.GetMock<IAccountRepository>()
                .Verify(method => method.RemoveTransactionAsync(It.IsAny<Transaction>()), Times.Never);

            _mock.GetMock<IUnitOfWork>()
                .Verify(method => method.CommitAsync(), Times.Never);
        }

        [Fact(DisplayName = "Alterar o valor de uma transação existente deve retornar sucesso.")]
        public async Task ExistsTransaction_SetTransactionValue_ReturnSuccess()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), "account", 1000, AccountType.CheckingAccount);
            var transactionId = Guid.NewGuid();
            var newTransactionValue = _faker.Finance.Amount(10, 100);
            account.AddTransaction(transactionId, DateTime.Now, 100, TransactionType.Income);
            var transaction = account.GetTransaction(transactionId);

            _mock.GetMock<IAccountRepository>()
                .Setup(method => method.GetTransactionByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(transaction);

            // Act
            await _service.SetTransactionValue(transactionId, newTransactionValue);

            // Assert
            Assert.Equal(newTransactionValue, transaction.Value.Value);

            _mock.GetMock<IAccountRepository>()
                .Verify(method => method.GetTransactionByIdAsync(It.IsAny<Guid>()), Times.Once);

            _mock.GetMock<IUnitOfWork>()
                .Verify(method => method.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Alterar o valor de uma transação inexistente não deve retornar erros, nem causar efeitos colaterais.")]
        public async Task NotFoundTransaction_SetTransactionValue_DoNothing()
        {
            // Arrange
            _mock.GetMock<IAccountRepository>()
                .Setup(method => method.GetTransactionByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Transaction)null);

            // Act
            await _service.SetTransactionValue(Guid.NewGuid(), _faker.Finance.Amount(0, 100));

            // Assert
            _mock.GetMock<IAccountRepository>()
                .Verify(method => method.GetTransactionByIdAsync(It.IsAny<Guid>()), Times.Once);

            _mock.GetMock<IUnitOfWork>()
                .Verify(method => method.CommitAsync(), Times.Never);
        }
    }
}