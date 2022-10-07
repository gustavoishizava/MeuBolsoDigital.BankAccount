using System;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Domain.Interfaces.Services;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.BankAccounts.Domain.Services
{
    public class UpdateTransactionValueService : IUpdateTransactionValueService
    {
        private readonly IAccountRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTransactionValueService(IAccountRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UpdateValueAsync(Guid transactionId, decimal value)
        {
            var transaction = await _repository.GetTransactionByIdAsync(transactionId);
            if (transaction is null)
                return false;

            transaction.SetValue(value);

            await _repository.UpdateTransactionAsync(transaction);
            return await _unitOfWork.CommitAsync();
        }
    }
}