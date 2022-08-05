using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Events;
using MediatR;

namespace MBD.BankAccounts.Application.DomainEvents
{
    public class DescriptionChangedDomainEventHandler : INotificationHandler<DescriptionChangedDomainEvent>
    {
        public Task Handle(DescriptionChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}