using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MongoDB.Driver;

namespace MBD.BankAccounts.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountContext _context;
        private readonly ILoggedUser _loggedUser;

        public AccountRepository(AccountContext context, ILoggedUser loggedUser)
        {
            _context = context;
            _loggedUser = loggedUser;
        }

        public async Task AddAsync(Account entity)
        {
            await _context.Accounts.AddAsync(entity);
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts.Collection.Find(x => x.TenantId == _loggedUser.UserId).ToListAsync();
        }

        public async Task<Account> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.Collection.Find(x => x.Id == id && x.TenantId == _loggedUser.UserId).FirstOrDefaultAsync();
        }

        public async Task<Account> GetByIdWithoutUserAsync(Guid id)
        {
            return await _context.Accounts.Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid transactionId)
        {
            return await _context.Transactions.Collection.Find(x => x.Id == transactionId).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(Account entity)
        {
            await _context.Accounts.RemoveAsync(Builders<Account>.Filter.Where(x => x.Id == entity.Id), entity);
        }

        public async Task RemoveTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.RemoveAsync(Builders<Transaction>.Filter.Where(x => x.Id == transaction.Id), transaction);
        }

        public async Task UpdateAsync(Account entity)
        {
            await _context.Accounts.UpdateAsync(Builders<Account>.Filter.Where(x => x.Id == entity.Id), entity);
        }
    }
}