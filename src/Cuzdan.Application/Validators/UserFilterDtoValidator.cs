using Cuzdan.Application.DTOs;
using FluentValidation;

namespace Cuzdan.Application.Validators
{
    public class UserFilterDtoValidator : AbstractValidator<UserFilterDto>
    {
        public UserFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Page size must be smaller than 100");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid e-mail format")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("The end date cannot be earlier than the start date.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("A valid user role must be specified.")
                .When(x => x.Role.HasValue);

            RuleFor(x => x.OrderBy)
                .IsInEnum().WithMessage("A valid sort field must be specified.")
                .When(x => x.OrderBy.HasValue);
        }
    }
}