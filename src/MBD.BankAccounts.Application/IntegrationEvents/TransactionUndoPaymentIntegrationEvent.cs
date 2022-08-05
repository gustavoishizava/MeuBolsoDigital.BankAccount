using System;

namespace MBD.BankAccounts.Application.IntegrationEvents
{
    public class TransactionUndoPaymentIntegrationEvent
    {
        public Guid Id { get; init; }
    }
}