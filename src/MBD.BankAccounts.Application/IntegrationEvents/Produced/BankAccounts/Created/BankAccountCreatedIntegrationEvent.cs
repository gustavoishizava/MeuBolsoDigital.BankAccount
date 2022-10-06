using System;
using MBD.BankAccounts.Domain.Enumerations;
using MediatR;

namespace MBD.BankAccounts.Application.IntegrationEvents.Produced.BankAccounts.Created
{
    public class BankAccountCreatedIntegrationEvent : INotification
    {
        public Guid Id { get; private init; }
        public Guid TenantId { get; private init; }
        public string Description { get; private init; }
        public string Type { get; private init; }

        public BankAccountCreatedIntegrationEvent(Guid id, Guid tenantId, string description, AccountType type)
        {
            Id = id;
            TenantId = tenantId;
            Description = description;
            Type = type.ToString();
        }
    }
}