using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Events;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog.Services;

namespace MBD.BankAccounts.Application.DomainEvents
{
    public class AccountCreatedDomainEventHandler : INotificationHandler<AccountCreatedDomainEvent>
    {
        private readonly IIntegrationEventLogService _service;

        public AccountCreatedDomainEventHandler(IIntegrationEventLogService service)
        {
            _service = service;
        }

        public async Task Handle(AccountCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _service.CreateEventAsync<AccountCreatedDomainEvent>(notification, "created");
        }
    }
}