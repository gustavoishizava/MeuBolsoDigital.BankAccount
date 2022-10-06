using System;
using MediatR;

namespace MBD.BankAccounts.Application.IntegrationEvents.Consumed.Transactions.UndoPayment
{
    public class TransactionUndoPaymentIntegrationEvent : INotification
    {
        public Guid Id { get; init; }
    }
}