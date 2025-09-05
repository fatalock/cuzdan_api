using Cuzdan.Application.DTOs;
using FluentValidation;

namespace Cuzdan.Application.Validators
{
    public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
    {
        public CreateTransactionDtoValidator()
        {
            RuleFor(x => x.FromId)
                .NotEmpty().WithMessage("Source wallet ID (FromId) is required.");

            RuleFor(x => x.ToId)
                .NotEmpty().WithMessage("Destination wallet ID (ToId) is required.")
                .NotEqual(x => x.FromId).WithMessage("Cannot transfer to the same wallet.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");
        }
    }
}