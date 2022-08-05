using System;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Enumerations;

namespace MBD.BankAccounts.Domain.Interfaces.Services
{
    public interface ITransactionManagementService
    {
        Task AddTransactionToAccountAsync(Guid accountId, Guid transactionId, decimal value, TransactionType type, DateTime createdAt);
        Task RemoveTransactionAsync(Guid transactionId);
        Task SetTransactionValue(Guid transactionId, decimal value);
    }
}