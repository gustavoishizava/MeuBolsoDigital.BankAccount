using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Infrastructure.Extensions;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog;
using MongoDB.Driver;

namespace MBD.BankAccounts.Infrastructure.Context
{
    [ExcludeFromCodeCoverage]
    public class AccountContext : DbContext
    {
        private readonly IMediator _mediator;

        public AccountContext(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options, IMediator mediator) : base(mongoClient, mongoDatabase, options)
        {
            _mediator = mediator;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<IntegrationEventLogEntry> IntegrationEventLogEntries { get; set; }

        public override async Task CommitAsync()
        {
            await _mediator.DispatchDomainEventsAsync(this);
            await base.CommitAsync();
        }
    }
}