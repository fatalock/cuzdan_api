using Cuzdan.Application.DTOs;
using FluentValidation;

namespace Cuzdan.Application.Validators
{
    public class TransactionFilterDtoValidator : AbstractValidator<TransactionFilterDto>
    {
        public TransactionFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot be greater than 100.");

            RuleFor(x => x.MinAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum amount cannot be negative.")
                .When(x => x.MinAmount.HasValue);

            RuleFor(x => x.MaxAmount)
                .GreaterThanOrEqualTo(x => x.MinAmount)
                .WithMessage("Maximum amount cannot be less than the minimum amount.")
                .When(x => x.MinAmount.HasValue && x.MaxAmount.HasValue);

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("The end date cannot be earlier than the start date.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("A valid transaction type must be specified.")
                .When(x => x.Type.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("A valid transaction status must be specified.")
                .When(x => x.Status.HasValue);
                
            RuleFor(x => x.OrderBy)
                .IsInEnum().WithMessage("Please specify a valid order.")
                .When(x => x.OrderBy.HasValue);
        }
    }
}