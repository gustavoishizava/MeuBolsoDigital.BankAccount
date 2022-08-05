using System;
using MBD.BankAccounts.Domain.Events.Common;

namespace MBD.BankAccounts.Domain.Events
{
    public class DescriptionChangedDomainEvent : DomainEvent
    {
        public Guid Id { get; private init; }
        public string OldDescription { get; private init; }
        public string NewDescription { get; private init; }

        public DescriptionChangedDomainEvent(Guid id, string oldDescription, string newDescription)
        {
            AggregateId = id;
            Id = id;
            OldDescription = oldDescription;
            NewDescription = newDescription;
        }
    }
}