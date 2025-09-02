using Cuzdan.Application.DTOs;
using FluentValidation;

namespace Cuzdan.Application.Validators
{
    public class WalletFilterDtoValidator : AbstractValidator<WalletFilterDto>
    {
        public WalletFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot be greater than 100.");

            RuleFor(x => x.MinBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum balance cannot be negative.")
                .When(x => x.MinBalance.HasValue);

            RuleFor(x => x.MaxBalance)
                .GreaterThanOrEqualTo(x => x.MinBalance)
                .WithMessage("Maximum balance cannot be less than the minimum balance.")
                .When(x => x.MinBalance.HasValue && x.MaxBalance.HasValue);

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("The end date cannot be earlier than the start date.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleFor(x => x.Currency)
                .IsInEnum().WithMessage("Please specify a valid currency.")
                .When(x => x.Currency.HasValue);

            RuleFor(x => x.OrderBy)
                .IsInEnum().WithMessage("Please specify a valid order.")
                .When(x => x.OrderBy.HasValue);
        }
    }
}