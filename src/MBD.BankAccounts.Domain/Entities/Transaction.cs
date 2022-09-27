using System;
using MBD.BankAccounts.Domain.Entities.ValueObjects;
using MBD.BankAccounts.Domain.Enumerations;

namespace MBD.BankAccounts.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private init; }
        public Guid AccountId { get; private init; }
        public DateTime CreatedAt { get; private set; }
        public Money Value { get; private set; }
        public TransactionType Type { get; private init; }

        internal Transaction(Guid id, Guid accountId, DateTime createdAt, decimal value, TransactionType type)
        {
            Id = id;
            AccountId = accountId;
            CreatedAt = createdAt;
            SetValue(value);
            Type = type;
        }

        internal void SetValue(decimal value)
        {
            Value = new Money(value);
        }

        internal void SetDate(DateTime date)
        {
            CreatedAt = date;
        }
    }
}