using System;

namespace MBD.BankAccounts.Domain.Entities.Common
{
    public class BaseEntity
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private init; }
        public DateTime? UpdatedAt { get; private set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        protected void SetUpdateDate() => UpdatedAt = DateTime.Now;
    }
}