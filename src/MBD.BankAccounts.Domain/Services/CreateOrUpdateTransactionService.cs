using System;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Domain.Interfaces.Services;
using MeuBolsoDigital.Core.Exceptions;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MBD.BankAccounts.Domain.Services
{
    public class CreateOrUpdateTransactionService : ICreateOrUpdateTransactionService
    {
        private readonly IAccountRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateOrUpdateTransactionService> _logger;

        public CreateOrUpdateTransactionService(IAccountRepository repository, IUnitOfWork unitOfWork, ILogger<CreateOrUpdateTransactionService> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> CreateOrUpdateAsync(Guid accountId, Guid transactionId, decimal value, TransactionType type, DateTime createdAt)
        {
            var account = await _repository.GetByIdWithoutUserAsync(accountId);
            if (account is null)
            {
                _logger.LogError($"Account not found. AccountId='{accountId}' | TransactionId='{transactionId}'");
                return true;
            }

            var transaction = await _repository.GetTransactionByIdAsync(transactionId);
            if (transaction is null)
            {
                account.AddTransaction(transactionId, createdAt, value, type);
                await _repository.UpdateAsync(account);

                return await _unitOfWork.CommitAsync();
            }

            await UpdateTransactionAsync(account, transaction, value, type, createdAt);
            return await _unitOfWork.CommitAsync();
        }

        private async Task UpdateTransactionAsync(Account account, Transaction transaction, decimal value, TransactionType type, DateTime createdAt)
        {
            if (ChangedAccount(transaction.AccountId, account.Id))
                await RemoveFromOldAccountAsync(transaction);

            if (account.GetTransaction(transaction.Id) is null)
                account.AddTransaction(transaction.Id, createdAt, value, type);
            else
                account.UpdateTransaction(transaction.Id, createdAt, value);

            await _repository.UpdateAsync(account);
        }

        private bool ChangedAccount(Guid currentAccountId, Guid newAccountId) => currentAccountId != newAccountId;

        private async Task RemoveFromOldAccountAsync(Transaction transaction)
        {
            var oldAccount = await _repository.GetByIdWithoutUserAsync(transaction.AccountId);
            if (oldAccount is null)
                throw new DomainException($"Old Account not found. AccountId='{transaction.AccountId}' | TransactionId='{transaction.Id}'");

            oldAccount.RemoveTransaction(transaction.Id);

            await _repository.RemoveTransactionAsync(transaction);
            await _repository.UpdateAsync(oldAccount);
        }
    }
}