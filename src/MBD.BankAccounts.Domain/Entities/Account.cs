using System;
using System.Collections.Generic;
using System.Linq;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Events;
using MeuBolsoDigital.Core.Assertions;
using MeuBolsoDigital.Core.Exceptions;
using MeuBolsoDigital.Core.Interfaces.Entities;

namespace MBD.BankAccounts.Domain.Entities
{
    public class Account : BaseEntityWithEvent, IAggregateRoot
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public Guid TenantId { get; private set; }
        public string Description { get; private set; }
        public decimal InitialBalance { get; private set; }
        public AccountType Type { get; private set; }
        public Status Status { get; private set; }

        #region Navigation

        public decimal Balance => InitialBalance + _transactions.Sum(x => x.Type == TransactionType.Income ? x.Value : x.Value * -1);
        public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

        #endregion

        public Account(Guid tenantId, string description, decimal initialBalance, AccountType type)
        {
            DomainAssertions.IsGreaterOrEqualsThan(initialBalance, 0, "O saldo inicial não pode ser inferiror a R$0,00.");
            DomainAssertions.IsNotNullOrEmpty(description, "A descrição deve ser informada.");
            DomainAssertions.HasMaxLength(description, 150, "A descrição deve conter no máximo 150 caracteres.");

            TenantId = tenantId;
            Description = description;
            InitialBalance = initialBalance;
            SetType(type);
            Activate();

            AddDomainEvent(new AccountCreatedDomainEvent(this));
        }

        protected Account()
        {
        }

        #region Account

        public void SetDescription(string description)
        {
            DomainAssertions.IsNotNullOrEmpty(description, "A descrição deve ser informada.");
            DomainAssertions.HasMaxLength(description, 150, "A descrição deve conter no máximo 150 caracteres.");

            if (!Description.Equals(description))
                AddDomainEvent(new DescriptionChangedDomainEvent(Id, Description, description));

            Description = description;
        }

        public void SetType(AccountType type)
        {
            Type = type;
        }

        public void Activate()
        {
            Status = Status.Active;
        }

        public void Deactivate()
        {
            Status = Status.Inactive;
        }

        #endregion

        #region Transactions

        public void AddTransaction(Guid transactionId, DateTime createdAt, decimal value, TransactionType type)
        {
            DomainAssertions.IsFalse(ExistingTransaction(transactionId), String.Format("Transação já existente. Id='{0}'.", transactionId));

            _transactions.Add(new Transaction(transactionId, Id, createdAt, value, type));
        }

        public Transaction GetTransaction(Guid transactionId)
        {
            return _transactions.Find(x => x.Id == transactionId);
        }

        public bool ExistingTransaction(Guid transactionId)
        {
            return _transactions.Any(x => x.Id == transactionId);
        }

        public void UpdateTransaction(Guid transactionId, DateTime createdAt, decimal value)
        {
            var transaction = GetTransaction(transactionId);
            if (transaction is null)
                throw new DomainException("Transação não encontrada.");

            transaction.SetValue(value);
            transaction.SetDate(createdAt);
        }

        #endregion
    }
}