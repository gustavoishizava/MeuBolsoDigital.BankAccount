using AutoMapper;
using MBD.BankAccounts.Application.Response;
using MBD.BankAccounts.Domain.Entities;

namespace MBD.BankAccounts.Application.AutoMapper
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Account, AccountResponse>(MemberList.Destination);
        }
    }
}