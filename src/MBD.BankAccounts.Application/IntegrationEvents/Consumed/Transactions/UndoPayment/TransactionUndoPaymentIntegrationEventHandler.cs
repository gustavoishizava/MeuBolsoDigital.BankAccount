using System.Threading;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.UndoPayment
{
    public class TransactionUndoPaymentIntegrationEventHandler : INotificationHandler<TransactionUndoPaymentIntegrationEvent>
    {
        private readonly IAccountRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionUndoPaymentIntegrationEventHandler(IAccountRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(TransactionUndoPaymentIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var transaction = await _repository.GetTransactionByIdAsync(notification.Id);
            if (transaction is null)
                return;

            await _repository.RemoveTransactionAsync(transaction);
            await _unitOfWork.CommitAsync();
        }
    }
}