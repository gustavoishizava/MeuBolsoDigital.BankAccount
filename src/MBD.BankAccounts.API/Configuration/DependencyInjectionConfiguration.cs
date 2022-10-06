using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MBD.BankAccounts.API.Identity;
using MBD.BankAccounts.API.Workers;
using MBD.BankAccounts.Application.DomainEvents;
using MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.Paid;
using MBD.BankAccounts.Application.Interfaces;
using MBD.BankAccounts.Application.Services;
using MBD.BankAccounts.Domain.Events;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MBD.BankAccounts.Domain.Interfaces.Services;
using MBD.BankAccounts.Domain.Services;
using MBD.BankAccounts.Infrastructure;
using MBD.BankAccounts.Infrastructure.Repositories;
using MediatR;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using MeuBolsoDigital.IntegrationEventLog.Extensions;
using MeuBolsoDigital.RabbitMQ.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MBD.BankAccounts.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDomaindServices()
                    .AddAppServices()
                    .AddRepositories()
                    .AddConsumers()
                    .AddConfigurations(configuration)
                    .AddDomainEvents()
                    .AddIntegrationEventLog<IntegrationEventLogRepository>()
                    .AddOutBoxTransaction()
                    .AddRabbitMqConnection(configuration);

            services.AddHttpContextAccessor();
            services.AddScoped<ILoggedUser, WebAppUser>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.Load("MBD.BankAccounts.Application"));

            return services;
        }

        public static IServiceCollection AddDomaindServices(this IServiceCollection services)
        {
            services.AddScoped<ICreateOrUpdateTransactionService, CreateOrUpdateTransactionService>();

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountAppService, AccountAppService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddConsumers(this IServiceCollection services)
        {
            services.AddHostedService<RabbitMqWorker>();

            return services;
        }

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddDomainEvents(this IServiceCollection services)
        {
            services.AddScoped<INotificationHandler<DescriptionChangedDomainEvent>, DescriptionChangedDomainEventHandler>();
            services.AddScoped<INotificationHandler<AccountCreatedDomainEvent>, AccountCreatedDomainEventHandler>();

            return services;
        }

        public static IServiceCollection AddOutBoxTransaction(this IServiceCollection services)
        {
            services.AddHostedService<IntegrationEventWorker>();

            return services;
        }

        public static IServiceCollection AddIntegrationEvents(this IServiceCollection services)
        {
            services.AddScoped<INotificationHandler<TransactionPaidIntegrationEvent>, TransactionPaidIntegrationEventHandler>();

            return services;
        }
    }
}