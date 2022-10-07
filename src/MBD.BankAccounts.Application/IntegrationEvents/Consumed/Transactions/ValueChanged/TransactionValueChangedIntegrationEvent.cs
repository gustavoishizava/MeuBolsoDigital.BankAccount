using System;
using MediatR;

namespace MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.ValueChanged
{
    public class TransactionValueChangedIntegrationEvent : INotification
    {
        public Guid Id { get; init; }
        public decimal NewValue { get; init; }
    }
}