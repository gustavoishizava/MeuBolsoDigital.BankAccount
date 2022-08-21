using System;
using MBD.BankAccounts.Domain.Enumerations;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.BankAccounts.Infrastructure.Context.CustomSerializers
{
    public class AccountTypeSerializer : SerializerBase<AccountType>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AccountType value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override AccountType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Enum.Parse<AccountType>(context.Reader.ReadString());
        }
    }
}