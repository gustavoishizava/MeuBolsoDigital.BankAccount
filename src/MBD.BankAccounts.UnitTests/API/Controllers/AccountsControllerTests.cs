using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using MBD.BankAccounts.API.Controllers;
using MBD.BankAccounts.API.Models;
using MBD.BankAccounts.Application.Interfaces;
using MBD.BankAccounts.Application.Request;
using MBD.BankAccounts.Application.Response;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Domain.Enumerations;
using MeuBolsoDigital.Application.Utils.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.BankAccounts.UnitTests.API.Controllers
{
    public class AccountsControllerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();
            _controller = _autoMocker.CreateInstance<AccountsController>();
        }

        [Fact]
        public async Task GetAll_Empty_ReturnNoContent()
        {
            // Arrante && Act
            var response = await _controller.GetAll() as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            _autoMocker.GetMock<IAccountAppService>().Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnOk()
        {
            // Arrange
            var account = new AccountResponse
            {
                Id = Guid.NewGuid(),
                Balance = _faker.Finance.Amount(),
                Description = _faker.Random.AlphaNumeric(10),
                Status = Status.Active,
                Type = AccountType.CheckingAccount
            };

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.GetAllAsync())
                       .ReturnsAsync(new List<AccountResponse> { account });

            // Act
            var response = await _controller.GetAll() as ObjectResult;
            var value = response.Value as List<AccountResponse>;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.NotNull(value);
            Assert.Equal(account, value.First());
            serviceMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.GetByIdAsync(id))
                       .ReturnsAsync(Result<AccountResponse>.Fail());

            // Act
            var response = await _controller.GetById(id) as NotFoundResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
            serviceMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();

            var account = new AccountResponse
            {
                Id = id,
                Balance = _faker.Finance.Amount(),
                Description = _faker.Random.AlphaNumeric(10),
                Status = Status.Active,
                Type = AccountType.CheckingAccount
            };

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.GetByIdAsync(id))
                       .ReturnsAsync(Result<AccountResponse>.Success(account));

            // Act
            var response = await _controller.GetById(id) as ObjectResult;
            var value = response.Value as AccountResponse;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(account, value);
            serviceMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnBadRequest()
        {
            // Arrange
            var request = new CreateAccountRequest
            {
                InitialBalance = _faker.Finance.Amount(),
                Description = _faker.Random.AlphaNumeric(10),
                Type = AccountType.CheckingAccount
            };

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.CreateAsync(request))
                       .ReturnsAsync(Result<AccountResponse>.Fail(errorMessage));

            // Act
            var response = await _controller.Create(request) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
            serviceMock.Verify(x => x.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnCreated()
        {
            // Arrange
            var request = new CreateAccountRequest
            {
                InitialBalance = _faker.Finance.Amount(),
                Description = _faker.Random.AlphaNumeric(10),
                Type = AccountType.CheckingAccount
            };

            var account = new AccountResponse
            {
                Id = Guid.NewGuid(),
                Balance = request.InitialBalance,
                Description = request.Description,
                Status = Status.Active,
                Type = request.Type
            };

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.CreateAsync(request))
                       .ReturnsAsync(Result<AccountResponse>.Success(account));

            // Act
            var response = await _controller.Create(request) as CreatedResult;
            var value = response.Value as AccountResponse;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal(account, value);
            Assert.Equal($"/api/accounts/{account.Id}", response.Location);
            serviceMock.Verify(x => x.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnBadRequest()
        {
            // Arrange
            var request = new UpdateAccountRequest
            {
                Id = Guid.NewGuid(),
                Description = _faker.Random.AlphaNumeric(10),
                Type = AccountType.CheckingAccount,
                Status = Status.Active
            };

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.UpdateAsync(request))
                       .ReturnsAsync(Result<AccountResponse>.Fail(errorMessage));

            // Act
            var response = await _controller.Update(request) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
            serviceMock.Verify(x => x.UpdateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnNoContent()
        {
            // Arrange
            var request = new UpdateAccountRequest
            {
                Id = Guid.NewGuid(),
                Description = _faker.Random.AlphaNumeric(10),
                Type = AccountType.CheckingAccount,
                Status = Status.Active
            };

            var account = new AccountResponse
            {
                Id = request.Id,
                Balance = _faker.Finance.Amount(),
                Description = request.Description,
                Status = request.Status,
                Type = request.Type
            };

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.UpdateAsync(request))
                       .ReturnsAsync(Result<AccountResponse>.Success(account));

            // Act
            var response = await _controller.Update(request) as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            serviceMock.Verify(x => x.UpdateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.RemoveAsync(id))
                       .ReturnsAsync(Result<AccountResponse>.Fail(errorMessage));

            // Act
            var response = await _controller.Delete(id) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
            serviceMock.Verify(x => x.RemoveAsync(id), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();

            var serviceMock = _autoMocker.GetMock<IAccountAppService>();

            serviceMock.Setup(x => x.RemoveAsync(id))
                       .ReturnsAsync(Result<AccountResponse>.Success());

            // Act
            var response = await _controller.Delete(id) as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            serviceMock.Verify(x => x.RemoveAsync(id), Times.Once);
        }
    }
}