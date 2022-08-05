using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Events;
using MediatR;

namespace MBD.BankAccounts.Application.DomainEvents
{
    public class AccountCreatedDomainEventHandler : INotificationHandler<AccountCreatedDomainEvent>
    {
        public Task Handle(AccountCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}