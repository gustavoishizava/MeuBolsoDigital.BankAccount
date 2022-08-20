using System;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Domain.Interfaces.Services;
using MeuBolsoDigital.Core.Exceptions;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.BankAccounts.Domain.Services
{
    public class TransactionManagementService : ITransactionManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;

        public TransactionManagementService(IUnitOfWork unitOfWork, IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
        }

        public async Task AddTransactionToAccountAsync(Guid accountId, Guid transactionId, decimal value, TransactionType type, DateTime createdAt)
        {
            var account = await _accountRepository.GetByIdAsync(accountId, true);
            if (account == null)
                throw new DomainException($"Nenhuma conta encontrada com o Id='{accountId}'.");

            var transaction = account.GetTransaction(transactionId);
            if (transaction is null)
                account.AddTransaction(transactionId, createdAt, value, type);
            else
                account.UpdateTransaction(transactionId, createdAt, value);

            await _accountRepository.UpdateAsync(account);

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveTransactionAsync(Guid transactionId)
        {
            var transaction = await _accountRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                return;

            _accountRepository.RemoveTransaction(transaction);
            await _unitOfWork.CommitAsync();
        }

        public async Task SetTransactionValue(Guid transactionId, decimal value)
        {
            var transaction = await _accountRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                return;

            transaction.SetValue(value);

            await _unitOfWork.CommitAsync();
        }
    }
}