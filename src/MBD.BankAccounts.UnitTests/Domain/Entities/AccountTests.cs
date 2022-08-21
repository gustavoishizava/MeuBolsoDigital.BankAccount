using System;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Domain.Enumerations;
using MeuBolsoDigital.Core.Exceptions;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Domain.Entities
{
    public class AccountTests
    {
        [Theory(DisplayName = "Criar conta com saldo e descrição inválida deve retornar Domain Exception.")]
        [InlineData("", 0)]
        [InlineData("Banco do Brasil", -1)]
        public void InvalidBalanceAndDescription_NewAccount_ReturnDomainException(string description, decimal initialBalance)
        {
            // Arrange && Act && Assert
            Assert.Throws<DomainException>(() =>
                new Account(Guid.NewGuid(), description, initialBalance, AccountType.CheckingAccount));
        }

        [Fact(DisplayName = "Criar conta com descrição excedendo o limite de tamanho deve retornar Domain Exception.")]
        public void InvalidDescriptionLength_NewAccount_ReturnDomainException()
        {
            // Arrange
            var invalidDescription = new String('a', 151);

            // Act && Assert
            Assert.Throws<DomainException>(() =>
                new Account(Guid.NewGuid(), invalidDescription, 0, AccountType.CheckingAccount));
        }

        [Theory(DisplayName = "Criar conta com descrição e saldo inicial válido deve retornar sucesso.")]
        [InlineData("Banco do Brasil", 0)]
        [InlineData("NuBank", 100)]
        [InlineData("Itaú", 5000)]
        [InlineData("Santander", 0.1)]
        public void ValidDescriptionAndBalance_NewAccount_ReturnSuccess(string description, decimal initialBalance)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var accountType = AccountType.CheckingAccount;

            // Act
            var account = new Account(tenantId, description, initialBalance, accountType);

            // Assert
            Assert.NotNull(account);
            Assert.Equal(tenantId, account.TenantId);
            Assert.Equal(description, account.Description);
            Assert.Equal(initialBalance, account.InitialBalance);
            Assert.Equal(AccountType.CheckingAccount, account.Type);
            Assert.Equal(Status.Active, account.Status);
        }

        [Fact(DisplayName = "Alterar uma conta bancária exitente com sucesso.")]
        public void ValidAccount_UpdateAccount_ReturnSuccess()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), "Conta existente", 100, AccountType.CheckingAccount);
            var newDescription = "Novo nome";
            var newType = AccountType.Money;
            var newStatus = Status.Inactive;
            account.ClearDomainEvents();

            // Act
            account.SetDescription(newDescription);
            account.SetType(newType);
            account.Deactivate();

            // Assert
            Assert.Single(account.Events);
            Assert.Equal(newDescription, account.Description);
            Assert.Equal(newType, account.Type);
            Assert.Equal(newStatus, account.Status);
            Assert.NotNull(account.UpdatedAt);
        }

        [Theory(DisplayName = "Atualizar descrição com valor inválido deve retornar DomainException.")]
        [InlineData(null)]
        [InlineData("")]
        public void InvalidDescription_SetDescription_ReturnDomainException(string description)
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), "Account", 100, AccountType.CheckingAccount);

            // Act && Assert
            Assert.Throws<DomainException>(() => account.SetDescription(description));
            Assert.Throws<DomainException>(() => account.SetDescription(new String('a', 151)));
        }
    }
}