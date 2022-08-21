using System;
using MBD.BankAccounts.Domain.Entities.Common;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Domain.Entities.Common
{
    public class BaseEntityTest
    {
        private class FakeEntity : BaseEntity
        {
            public void Update() => base.SetUpdateDate();
        }

        [Fact]
        public void Create_ReturnSuccess()
        {
            // Arrange && Act
            var entity = new FakeEntity();

            // Assert
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.NotEqual(DateTime.MinValue, entity.CreatedAt);
            Assert.Null(entity.UpdatedAt);
        }

        [Fact]
        public void SetUpdate_ReturnNewDate()
        {
            // Arrange
            var entity = new FakeEntity();
            var oldUpdatedAt = entity.UpdatedAt;

            // Act
            entity.Update();

            // Assert
            Assert.NotNull(entity.UpdatedAt);
            Assert.NotEqual(oldUpdatedAt, entity.UpdatedAt);
        }
    }
}