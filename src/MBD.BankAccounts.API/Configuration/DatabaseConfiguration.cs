using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Extensions;
using MBD.BankAccounts.Infrastructure.Context;
using MBD.BankAccounts.Infrastructure.Context.CustomSerializers;
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
                options.AddSerializer(new StatusSerializer());
                options.AddSerializer(new AccountTypeSerializer());
                options.AddSerializer(new StateSerializer());
                options.AddSerializer(new MoneySerializer());
            });

            return services;
        }
    }
}