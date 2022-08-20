using System.Threading.Tasks;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.Context.ModelConfiguration;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Infrastructure.Extensions;
using MediatR;

namespace MBD.BankAccounts.Infrastructure.Context
{
    public class AccountContext : DbContext
    {
        private readonly IMediator _mediator;

        public AccountContext(MongoDbContextOptions options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelConfiguring(ModelBuilder modelBuilder)
        {
            modelBuilder.AddModelMap<BaseEntity>(map =>
            {
                map.SetIsRootClass(true);

                map.MapIdProperty(x => x.Id);

                map.MapProperty(x => x.CreatedAt)
                    .SetElementName("created_at");

                map.MapProperty(x => x.UpdatedAt)
                    .SetElementName("updated_at");
            });

            modelBuilder.AddModelMap<BaseEntityWithEvent>();

            modelBuilder.AddModelMap<Account>("accounts", mapConfig =>
            {
                mapConfig.MapProperty(x => x.TenantId).SetElementName("tenant_id");
                mapConfig.MapProperty(x => x.Description).SetElementName("description");
                mapConfig.MapProperty(x => x.InitialBalance).SetElementName("initial_balance");
                mapConfig.MapProperty(x => x.Type).SetElementName("type");
                mapConfig.MapProperty(x => x.Status).SetElementName("status");
            });

            modelBuilder.AddModelMap<Transaction>("transactions", mapConfig =>
            {
                mapConfig.MapIdProperty(x => x.Id);
                mapConfig.MapProperty(x => x.AccountId).SetElementName("account_id");
                mapConfig.MapProperty(x => x.CreatedAt).SetElementName("created_at");
                mapConfig.MapProperty(x => x.Value).SetElementName("value");
                mapConfig.MapProperty(x => x.Type).SetElementName("type");
            });
        }

        public override Task CommitAsync()
        {
            _mediator.DispatchDomainEventsAsync(this);
            return base.CommitAsync();
        }
    }
}