using MeuBolsoDigital.Core.Assertions;

namespace MBD.BankAccounts.Domain.Entities.ValueObjects
{
    public record Money
    {
        public decimal Value { get; private init; }

        public Money(decimal value)
        {
            DomainAssertions.IsGreaterOrEqualsThan(value, 0, $"{nameof(value)} não pode ser inferiror a R$0,00.");
            Value = value;
        }
    }
}