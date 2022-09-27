using System;
using System.Diagnostics.CodeAnalysis;
using MBD.BankAccounts.Domain.Entities.ValueObjects;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.BankAccounts.Infrastructure.Context.CustomSerializers
{
    [ExcludeFromCodeCoverage]
    public class MoneySerializer : SerializerBase<Money>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Money value)
        {
            context.Writer.WriteDecimal128(value.Value);
        }

        public override Money Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new Money(Convert.ToDecimal(context.Reader.ReadDecimal128()));
        }
    }
}