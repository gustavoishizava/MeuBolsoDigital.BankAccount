using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Application.DomainEvents;
using MBD.BankAccounts.Application.IntegrationEvents.Produced.BankAccounts.Created;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Events;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Application.DomainEvents
{
    public class AccountCreatedDomainEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly AccountCreatedDomainEventHandler _handler;

        public AccountCreatedDomainEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _handler = _autoMocker.CreateInstance<AccountCreatedDomainEventHandler>();
        }

        [Fact]
        public async Task Handler_ReturnSuccess()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), "account", 100, AccountType.CheckingAccount);
            var @event = new AccountCreatedDomainEvent(account);

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
                .Verify(x => x.CreateEventAsync<BankAccountCreatedIntegrationEvent>(It.Is<BankAccountCreatedIntegrationEvent>(x => x.Id == @event.Id
                                                                                                                                   && x.TenantId == @event.TenantId
                                                                                                                                   && x.Description == @event.Description
                                                                                                                                   && x.Type == @event.Type.ToString()), "created"));
        }
    }
}