using System;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Events.Common;

namespace MBD.BankAccounts.Domain.Events
{
    public class AccountCreatedDomainEvent : DomainEvent
    {
        public Guid Id { get; private init; }
        public Guid TenantId { get; private init; }
        public string Description { get; private init; }
        public AccountType Type { get; private init; }

        public AccountCreatedDomainEvent(Account account)
        {
            AggregateId = account.Id;
            Id = account.Id;
            TenantId = account.TenantId;
            Description = account.Description;
            Type = account.Type;
        }
    }
}