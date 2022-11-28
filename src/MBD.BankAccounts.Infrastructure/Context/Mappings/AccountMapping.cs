using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.BankAccounts.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.BankAccounts.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class AccountMapping : BsonClassMapConfiguration
    {
        public AccountMapping() : base("accounts")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<Account>();

            map.MapProperty(x => x.TenantId).SetElementName("tenant_id");
            map.MapProperty(x => x.Description).SetElementName("description");
            map.MapProperty(x => x.InitialBalance).SetElementName("initial_balance");
            map.MapProperty(x => x.Type).SetElementName("type");
            map.MapProperty(x => x.Status).SetElementName("status");
            map.MapField("_transactions").SetShouldSerializeMethod(x => false);

            return map;
        }
    }
}