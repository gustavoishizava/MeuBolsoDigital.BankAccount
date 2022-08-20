using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Infrastructure.Context;

namespace MBD.BankAccounts.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountContext _context;

        public AccountRepository(AccountContext context)
        {
            _context = context;
        }

        public Task AddAsync(Account entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Account>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetByIdAsync(Guid id, bool ignoreGlobalFilter)
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> GetTransactionByIdAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Account entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Account entity)
        {
            throw new NotImplementedException();
        }
    }
}