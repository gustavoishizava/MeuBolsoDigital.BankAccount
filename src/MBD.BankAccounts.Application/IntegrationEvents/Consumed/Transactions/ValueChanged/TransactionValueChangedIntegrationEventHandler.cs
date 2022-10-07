using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Interfaces.Services;
using MediatR;

namespace MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.ValueChanged
{
    public class TransactionValueChangedIntegrationEventHandler : INotificationHandler<TransactionValueChangedIntegrationEvent>
    {
        private readonly IUpdateTransactionValueService _service;

        public TransactionValueChangedIntegrationEventHandler(IUpdateTransactionValueService service)
        {
            _service = service;
        }

        public async Task Handle(TransactionValueChangedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            await _service.UpdateValueAsync(notification.Id, notification.NewValue);
        }
    }
}