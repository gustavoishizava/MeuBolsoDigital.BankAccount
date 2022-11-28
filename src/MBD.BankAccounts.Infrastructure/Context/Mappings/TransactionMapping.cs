using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.BankAccounts.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.BankAccounts.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class TransactionMapping : BsonClassMapConfiguration
    {
        public TransactionMapping() : base("transactions")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<Transaction>();

            map.MapIdProperty(x => x.Id);
            map.MapProperty(x => x.AccountId).SetElementName("account_id");
            map.MapProperty(x => x.CreatedAt).SetElementName("created_at");
            map.MapProperty(x => x.Value).SetElementName("value");
            map.MapProperty(x => x.Type).SetElementName("type");

            return map;
        }
    }
}