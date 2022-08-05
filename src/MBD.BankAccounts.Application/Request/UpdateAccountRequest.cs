using System;
using FluentValidation;
using FluentValidation.Results;
using MBD.BankAccounts.Domain.Entities.Common;
using MBD.BankAccounts.Domain.Enumerations;

namespace MBD.BankAccounts.Application.Request
{
    public class UpdateAccountRequest : BaseRequest
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public AccountType Type { get; set; }
        public Status Status { get; set; }

        public override ValidationResult Validate()
        {
            return new UpdateAccountValidation().Validate(this);
        }

        public class UpdateAccountValidation : AbstractValidator<UpdateAccountRequest>
        {
            public UpdateAccountValidation()
            {
                RuleFor(x => x.Id)
                    .NotEmpty();

                RuleFor(x => x.Description)
                    .NotEmpty()
                    .MaximumLength(150);

                RuleFor(x => x.Type)
                    .IsInEnum();

                RuleFor(x => x.Status)
                    .IsInEnum();
            }
        }
    }
}