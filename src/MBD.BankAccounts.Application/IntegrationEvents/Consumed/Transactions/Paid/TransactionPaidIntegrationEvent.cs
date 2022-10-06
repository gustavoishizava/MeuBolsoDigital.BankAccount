using System;
using MediatR;

namespace MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.Paid
{
    public class TransactionPaidIntegrationEvent : INotification
    {
        public Guid Id { get; init; }
        public Guid BankAccountId { get; init; }
        public string Type { get; init; }
        public decimal Value { get; init; }
        public DateTime Date { get; init; }
    }
}