using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MBD.BankAccounts.Application.Interfaces;
using MBD.BankAccounts.Application.Request;
using MBD.BankAccounts.Application.Response;
using MBD.BankAccounts.Domain.Entities;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.BankAccounts.Application.Services
{
    public class AccountAppService : IAccountAppService
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly IAccountRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountAppService(IAspNetUser aspNetUser, IAccountRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _aspNetUser = aspNetUser;
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<AccountResponse>> CreateAsync(CreateAccountRequest request)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result<AccountResponse>.Fail(validation.ToString());

            var account = new Account(_aspNetUser.UserId, request.Description, request.InitialBalance, request.Type);

            _repository.Add(account);
            await _unitOfWork.CommitAsync();

            return Result<AccountResponse>.Success(_mapper.Map<AccountResponse>(account));
        }

        public async Task<IResult> UpdateAsync(UpdateAccountRequest request)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result.Fail(validation.ToString());

            var account = await _repository.GetByIdAsync(request.Id);
            if (account == null)
                return Result.Fail("Conta bancária inválida.");

            account.SetDescription(request.Description);
            account.SetType(request.Type);

            if (request.Status == Status.Active)
                account.Activate();
            else
                account.Deactivate();

            _repository.Update(account);
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }

        public async Task<IResult<AccountResponse>> GetByIdAsync(Guid id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null)
                return Result<AccountResponse>.Fail();

            return Result<AccountResponse>.Success(_mapper.Map<AccountResponse>(account));
        }

        public async Task<IEnumerable<AccountResponse>> GetAllAsync()
        {
            return _mapper.Map<IEnumerable<AccountResponse>>(await _repository.GetAllAsync());
        }

        public async Task<IResult> RemoveAsync(Guid id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null)
                return Result.Fail("Conta bancária inválida.");

            _repository.Remove(account);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}