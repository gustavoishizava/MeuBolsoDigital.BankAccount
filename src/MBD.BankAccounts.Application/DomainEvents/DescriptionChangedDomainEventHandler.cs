using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Events;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog.Services;

namespace MBD.BankAccounts.Application.DomainEvents
{
    public class DescriptionChangedDomainEventHandler : INotificationHandler<DescriptionChangedDomainEvent>
    {
        private readonly IIntegrationEventLogService _service;

        public DescriptionChangedDomainEventHandler(IIntegrationEventLogService service)
        {
            _service = service;
        }

        public async Task Handle(DescriptionChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _service.CreateEventAsync<DescriptionChangedDomainEvent>(notification, "updated.description");
        }
    }
}