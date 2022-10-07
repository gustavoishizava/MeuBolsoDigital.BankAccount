using System;
using System.Threading.Tasks;

namespace MBD.BankAccounts.Domain.Interfaces.Services
{
    public interface IUpdateTransactionValueService
    {
        Task<bool> UpdateValueAsync(Guid transactionId, decimal value);
    }
}