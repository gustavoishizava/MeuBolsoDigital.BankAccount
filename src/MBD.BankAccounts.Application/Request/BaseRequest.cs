using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace MBD.BankAccounts.Application.Request
{
    public abstract class BaseRequest
    {
        public abstract ValidationResult Validate();
    }
}