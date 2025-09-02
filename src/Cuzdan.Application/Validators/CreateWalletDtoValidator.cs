using Cuzdan.Application.DTOs;
using FluentValidation;

namespace Cuzdan.Application.Validators
{
    public class CreateWalletDtoValidator : AbstractValidator<CreateWalletDto>
    {
        public CreateWalletDtoValidator()
        {
            RuleFor(x => x.WalletName)
                .NotEmpty().WithMessage("Wallet name cannot be empty.")
                .MaximumLength(50).WithMessage("Wallet name cannot be longer than 50 characters.");

            RuleFor(x => x.Currency)
                .IsInEnum().WithMessage("Please specify a valid currency.");
        }
    }
}