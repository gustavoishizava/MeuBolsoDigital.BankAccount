using FluentValidation.Results;

namespace MBD.BankAccounts.Application.Request
{
    public abstract class BaseRequest
    {
        public abstract ValidationResult Validate();
    }
}