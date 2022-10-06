using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Application.IntegrationEvents.Produced.BankAccounts.Created;
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
            var @event = new BankAccountCreatedIntegrationEvent(notification.Id,
                                                                notification.TenantId,
                                                                notification.Description,
                                                                notification.Type);

            await _service.CreateEventAsync<BankAccountCreatedIntegrationEvent>(@event, "created");
        }
    }
}