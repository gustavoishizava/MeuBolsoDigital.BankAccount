using Bogus;
using MBD.BankAccounts.Domain.Entities.ValueObjects;
using MeuBolsoDigital.Core.Exceptions;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Domain.Entities.ValueObjects
{
    public class MoneyTests
    {
        private readonly Faker _faker;

        public MoneyTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void InvalidValue_ShouldReturnDomainException()
        {
            // Arrange
            var value = _faker.Random.Decimal(-10000, -1);

            // Act
            var exception = Assert.Throws<DomainException>(() => new Money(value));

            // Assert
            Assert.Equal("value não pode ser inferiror a R$0,00.", exception.Message);
        }

        [Fact]
        public void ValidValue_Success()
        {
            // Arrange
            var value = _faker.Random.Decimal(0, 10000);

            // Act
            var money = new Money(value);

            // Assert
            Assert.Equal(value, money.Value);
        }
    }
}