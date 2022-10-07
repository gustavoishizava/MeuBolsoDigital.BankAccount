using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Entities;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.BankAccounts.Domain.Interfaces.Repositories
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account> GetByIdWithoutUserAsync(Guid id);
        Task<Transaction> GetTransactionByIdAsync(Guid transactionId);
        Task RemoveTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
    }
}