using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MeuBolsoDigital.IntegrationEventLog;
using MongoDB.Bson.Serialization;

namespace MBD.BankAccounts.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class IntegrationEventLogEntryMapping : BsonClassMapConfiguration
    {
        public IntegrationEventLogEntryMapping() : base("integration_event_log_entries")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<IntegrationEventLogEntry>();

            map.MapIdProperty(x => x.Id);
            map.MapProperty(x => x.CreatedAt).SetElementName("created_at");
            map.MapProperty(x => x.UpdatedAt).SetElementName("updated_at");
            map.MapProperty(x => x.EventTypeName).SetElementName("entity_type_name");
            map.MapProperty(x => x.Content).SetElementName("content");
            map.MapProperty(x => x.State).SetElementName("state");

            return map;
        }
    }
}