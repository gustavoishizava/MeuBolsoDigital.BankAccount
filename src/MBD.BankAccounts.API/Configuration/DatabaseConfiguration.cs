using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Extensions;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Infrastructure.Context;
using MBD.BankAccounts.Infrastructure.Context.CustomSerializers;
using MeuBolsoDigital.IntegrationEventLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.BankAccounts.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext<AccountContext>(options =>
            {
                options.ConfigureConnection(configuration.GetConnectionString("Default"), configuration["DatabaseName"]);
                options.AddSerializer(new GuidSerializer(BsonType.String));
                options.AddSerializer(new MoneySerializer());
                options.AddSerializer(new EnumSerializer<TransactionType>(BsonType.String));
                options.AddSerializer(new EnumSerializer<EventState>(BsonType.String));
                options.AddSerializer(new EnumSerializer<AccountType>(BsonType.String));
                options.AddSerializer(new EnumSerializer<Status>(BsonType.String));
            });

            return services;
        }
    }
}