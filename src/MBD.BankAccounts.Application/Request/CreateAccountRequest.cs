using FluentValidation;
using FluentValidation.Results;
using MBD.BankAccounts.Domain.Enumerations;

namespace MBD.BankAccounts.Application.Request
{
    public class CreateAccountRequest : BaseRequest
    {
        public string Description { get; set; }
        public decimal InitialBalance { get; set; }
        public AccountType Type { get; set; }

        public override ValidationResult Validate()
        {
            return new CreateAccountValidation().Validate(this);
        }

        public class CreateAccountValidation : AbstractValidator<CreateAccountRequest>
        {
            public CreateAccountValidation()
            {
                RuleFor(x => x.Description)
                    .NotEmpty()
                    .MaximumLength(150);

                RuleFor(x => x.InitialBalance)
                    .NotEmpty()
                    .GreaterThan(0);

                RuleFor(x => x.Type)
                    .IsInEnum();
            }
        }
    }
}