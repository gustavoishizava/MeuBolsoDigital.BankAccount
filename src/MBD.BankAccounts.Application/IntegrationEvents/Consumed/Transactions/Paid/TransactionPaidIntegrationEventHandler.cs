using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Services;
using MediatR;

namespace MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.Paid
{
    public class TransactionPaidIntegrationEventHandler : INotificationHandler<TransactionPaidIntegrationEvent>
    {
        private readonly ICreateOrUpdateTransactionService _service;

        public TransactionPaidIntegrationEventHandler(ICreateOrUpdateTransactionService service)
        {
            _service = service;
        }

        public async Task Handle(TransactionPaidIntegrationEvent notification, CancellationToken cancellationToken)
        {
            await _service.CreateOrUpdateAsync(notification.BankAccountId,
                                               notification.Id,
                                               notification.Value,
                                               Enum.Parse<TransactionType>(notification.Type),
                                               notification.Date);
        }
    }
}