using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using MBD.BankAccounts.Application.AutoMapper;
using MBD.BankAccounts.Application.Interfaces;
using MBD.BankAccounts.Application.Request;
using MBD.BankAccounts.Application.Services;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Domain.Enumerations;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.Application.Services
{
    public class AccountAppServiceTests
    {
        private readonly IAccountAppService _appService;
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;

        public AccountAppServiceTests()
        {
            _autoMocker = new AutoMocker();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
            _autoMocker.Use<IMapper>(config.CreateMapper());

            _appService = _autoMocker.CreateInstance<AccountAppService>();
            _faker = new Faker();
        }

        [Fact]
        public async Task Create_ReturnFail()
        {
            // Arrange
            var request = new CreateAccountRequest
            {
                Description = string.Empty,
                InitialBalance = -10,
                Type = AccountType.CheckingAccount
            };

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            var unitOfWorkMock = _autoMocker.GetMock<IUnitOfWork>();

            // Act
            var result = await _appService.CreateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);
            Assert.Null(result.Data);
            repositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>()), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Create_ReturnSuccess()
        {
            // Arrange
            var request = new CreateAccountRequest
            {
                Description = _faker.Random.AlphaNumeric(150),
                InitialBalance = _faker.Finance.Amount(),
                Type = AccountType.CheckingAccount
            };

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            var unitOfWorkMock = _autoMocker.GetMock<IUnitOfWork>();

            // Act
            var result = await _appService.CreateAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(request.Description, result.Data.Description);
            Assert.Equal(request.InitialBalance, result.Data.Balance);
            Assert.Equal(request.Type, result.Data.Type);
            repositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnFail()
        {
            // Arrange
            var request = new UpdateAccountRequest
            {
                Id = Guid.Empty,
                Description = string.Empty,
                Status = Status.Active,
                Type = AccountType.CheckingAccount
            };

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            var unitOfWorkMock = _autoMocker.GetMock<IUnitOfWork>();

            // Act
            var result = await _appService.UpdateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);
            repositoryMock.Verify(x => x.GetByIdAsync(request.Id), Times.Never);
            repositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>()), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Update_AccountNotFound_ReturnFail()
        {
            // Arrange
            var request = new UpdateAccountRequest
            {
                Id = Guid.NewGuid(),
                Description = _faker.Random.AlphaNumeric(150),
                Status = Status.Active,
                Type = AccountType.CheckingAccount
            };

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            var unitOfWorkMock = _autoMocker.GetMock<IUnitOfWork>();

            repositoryMock.Setup(x => x.GetByIdAsync(request.Id))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _appService.UpdateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);
            repositoryMock.Verify(x => x.GetByIdAsync(request.Id), Times.Once);
            repositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>()), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Theory]
        [InlineData(Status.Active)]
        [InlineData(Status.Inactive)]
        public async Task Update_ReturnSuccess(Status status)
        {
            // Arrange
            var request = new UpdateAccountRequest
            {
                Id = Guid.NewGuid(),
                Description = _faker.Random.AlphaNumeric(150),
                Status = status,
                Type = AccountType.CheckingAccount
            };

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            var unitOfWorkMock = _autoMocker.GetMock<IUnitOfWork>();

            repositoryMock.Setup(x => x.GetByIdAsync(request.Id))
                .ReturnsAsync(new Account(Guid.NewGuid(), request.Description, 10, request.Type));

            // Act
            var result = await _appService.UpdateAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Message);
            repositoryMock.Verify(x => x.GetByIdAsync(request.Id), Times.Once);
            repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Account>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            repositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Account)null);

            // Act
            var result = await _appService.GetByIdAsync(id);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Message);
            repositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnSuccess()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), _faker.Random.AlphaNumeric(150), _faker.Finance.Amount(), AccountType.CheckingAccount);

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            repositoryMock.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

            // Act
            var result = await _appService.GetByIdAsync(account.Id);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(account.Description, result.Data.Description);
            Assert.Equal(account.InitialBalance.Value, result.Data.Balance);
            Assert.Equal(account.Type, result.Data.Type);
            repositoryMock.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnSuccess()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), _faker.Random.AlphaNumeric(150), _faker.Finance.Amount(), AccountType.CheckingAccount);

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Account> { account });

            // Act
            var result = await _appService.GetAllAsync();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Remove_AccountNotFound_ReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            var unitOfWorkMock = _autoMocker.GetMock<IUnitOfWork>();

            repositoryMock.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _appService.RemoveAsync(id);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);
            repositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            repositoryMock.Verify(x => x.RemoveAsync(It.IsAny<Account>()), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Remove_ReturnSuccess()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), _faker.Random.AlphaNumeric(150), _faker.Finance.Amount(), AccountType.CheckingAccount);

            var repositoryMock = _autoMocker.GetMock<IAccountRepository>();
            var unitOfWorkMock = _autoMocker.GetMock<IUnitOfWork>();

            repositoryMock.Setup(x => x.GetByIdAsync(account.Id))
                .ReturnsAsync(account);

            // Act
            var result = await _appService.RemoveAsync(account.Id);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Message);
            repositoryMock.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
            repositoryMock.Verify(x => x.RemoveAsync(It.IsAny<Account>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}