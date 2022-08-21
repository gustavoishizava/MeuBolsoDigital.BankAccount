using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MBD.BankAccounts.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.BankAccounts.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AccountContext _context;

        public UnitOfWork(AccountContext context)
        {
            _context = context;
            _context.StartTransaction();
        }

        public async Task<bool> CommitAsync()
        {
            await _context.CommitAsync();
            return true;
        }
    }
}