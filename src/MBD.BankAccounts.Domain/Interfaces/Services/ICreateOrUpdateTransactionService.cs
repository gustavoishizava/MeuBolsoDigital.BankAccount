using System;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Enumerations;

namespace MBD.BankAccounts.Domain.Interfaces.Services
{
    public interface ICreateOrUpdateTransactionService
    {
        Task<bool> CreateOrUpdateAsync(Guid accountId, Guid transactionId, decimal value, TransactionType type, DateTime createdAt);
    }
}