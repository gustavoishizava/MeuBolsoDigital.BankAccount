using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.MongoDB.Context.Context.Operations;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.CrossCutting.Extensions;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace MBD.BankAccounts.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
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
            await AddOrUpdateTransactionsAsync(entity.Transactions.ToList());
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts.Collection.Aggregate<Account>()
                                                    .Match(x => x.TenantId == _loggedUser.UserId)
                                                    .Lookup<Transaction, Account>(
                                                        foreignCollectionName: "transactions",
                                                        localField: "_id",
                                                        foreignField: "account_id",
                                                        @as: "_transactions").ToListAsync();
        }

        public async Task<Account> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.Collection.Aggregate<Account>()
                                                    .Match(x => x.Id == id && x.TenantId == _loggedUser.UserId)
                                                    .Lookup<Transaction, Account>(
                                                        foreignCollectionName: "transactions",
                                                        localField: "_id",
                                                        foreignField: "account_id",
                                                        @as: "_transactions").FirstOrDefaultAsync();
        }

        public async Task<Account> GetByIdWithoutUserAsync(Guid id)
        {
            return await _context.Accounts.Collection.Aggregate<Account>()
                                                    .Match(x => x.Id == id)
                                                    .Lookup<Transaction, Account>(
                                                        foreignCollectionName: "transactions",
                                                        localField: "_id",
                                                        foreignField: "account_id",
                                                        @as: "_transactions").FirstOrDefaultAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid transactionId)
        {
            return await _context.Transactions.Collection.Find(x => x.Id == transactionId).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(Account entity)
        {
            await _context.Accounts.RemoveAsync(Builders<Account>.Filter.Where(x => x.Id == entity.Id), entity);
            await RemoveTransactionByAccountIdAsync(entity.Id);
        }

        public async Task RemoveTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.RemoveAsync(Builders<Transaction>.Filter.Where(x => x.Id == transaction.Id), transaction);
        }

        public async Task UpdateAsync(Account entity)
        {
            await _context.Accounts.UpdateAsync(Builders<Account>.Filter.Where(x => x.Id == entity.Id), entity);
            await AddOrUpdateTransactionsAsync(entity.Transactions.ToList());
        }

        private async Task AddOrUpdateTransactionsAsync(List<Transaction> transactions)
        {
            if (transactions.IsNullOrEmpty())
                return;

            var bulkOperations = new List<BulkOperationModel<Transaction>>();
            foreach (var transaction in transactions)
                bulkOperations.Add(new(Builders<Transaction>.Filter.Where(x => x.Id == transaction.Id), transaction));

            await _context.Transactions.UpdateRangeAsync(bulkOperations);
        }

        private async Task RemoveTransactionByAccountIdAsync(Guid accountId)
        {
            await _context.Transactions.Collection.DeleteManyAsync(_context.ClientSessionHandle, Builders<Transaction>.Filter.Where(x => x.AccountId == accountId));
        }
    }
}