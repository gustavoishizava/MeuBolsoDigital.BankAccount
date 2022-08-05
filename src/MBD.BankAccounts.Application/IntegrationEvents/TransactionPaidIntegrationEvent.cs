using System;

namespace MBD.BankAccounts.Application.IntegrationEvents
{
    public class TransactionPaidIntegrationEvent
    {
        public Guid Id { get; init; }
        public Guid BankAccountId { get; init; }
        public string Type { get; init; }
        public decimal Value { get; init; }
        public DateTime Date { get; init; }
    }
}