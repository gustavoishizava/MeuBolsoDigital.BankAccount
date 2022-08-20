using System;
using MBD.BankAccounts.Domain.Entities.Common;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.BankAccounts.Infrastructure.Context.CustomSerializers
{
    public class StatusSerializer : SerializerBase<Status>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Status value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override Status Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var value = context.Reader.ReadString();
            return Enum.Parse<Status>(value);
        }
    }
}