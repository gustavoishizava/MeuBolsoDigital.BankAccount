using System.Collections.Generic;
using MBD.BankAccounts.Domain.Events.Common;

namespace MBD.BankAccounts.Domain.Entities.Common
{
    public abstract class BaseEntityWithEvent : BaseEntity
    {
        private List<DomainEvent> _events;
        public IReadOnlyList<DomainEvent> Events => _events?.AsReadOnly();

        public void AddDomainEvent(DomainEvent @event)
        {
            _events = _events ?? new();
            _events.Add(@event);
        }

        public void RemoveDomainEvent(DomainEvent @event)
        {
            _events?.Remove(@event);
        }

        public void ClearDomainEvents()
        {
            _events?.Clear();
        }
    }
}