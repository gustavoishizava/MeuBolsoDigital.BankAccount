using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MBD.BankAccounts.Infrastructure.Context;
using MeuBolsoDigital.IntegrationEventLog;
using MeuBolsoDigital.IntegrationEventLog.Repositories;
using MongoDB.Driver;

namespace MBD.BankAccounts.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class IntegrationEventLogRepository : IIntegrationEventLogRepository
    {
        private readonly AccountContext _context;

        public IntegrationEventLogRepository(AccountContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IntegrationEventLogEntry integrationEventLogEntry)
        {
            await _context.IntegrationEventLogEntries.AddAsync(integrationEventLogEntry);
        }

        public async Task<IntegrationEventLogEntry> FindNextToPublishAsync()
        {
            var update = Builders<IntegrationEventLogEntry>.Update
                            .Set(x => x.State, EventState.InProgress)
                            .Set(x => x.UpdatedAt, DateTime.Now);

            var filter = Builders<IntegrationEventLogEntry>.Filter.Where(x => x.State == EventState.NotPublished);

            return await _context.IntegrationEventLogEntries.Collection.FindOneAndUpdateAsync(filter, update);
        }

        public Task ResetFailedAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task ResetInProgressAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync()
        {
            var filter = Builders<IntegrationEventLogEntry>.Filter.Where(x => x.State == EventState.NotPublished);
            return await _context.IntegrationEventLogEntries.Collection.Find(filter).ToListAsync();
        }

        public async Task UpdateAsync(IntegrationEventLogEntry integrationEventLogEntry)
        {
            var filter = Builders<IntegrationEventLogEntry>.Filter.Where(x => x.Id == integrationEventLogEntry.Id);
            await _context.IntegrationEventLogEntries.UpdateAsync(filter, integrationEventLogEntry);
        }
    }
}