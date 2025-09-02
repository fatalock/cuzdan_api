using Cuzdan.Application.DTOs;
using FluentValidation;

namespace Cuzdan.Application.Validators
{
    public class DepositWithdrawalRequestDtoValidator : AbstractValidator<DepositWithdrawalRequestDto>
    {
        public DepositWithdrawalRequestDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.")
                .LessThanOrEqualTo(1000000).WithMessage("Amount cannot be greater than 1,000,000.");
        }
    }
}