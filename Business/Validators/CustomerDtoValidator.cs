using Business.Dtos;
using FluentValidation;
using System;

namespace Business.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(50);
            RuleFor(x => x.Telephone).Matches(@"^\d{9,12}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Telephone));
            RuleFor(x => x.Birthday)
                .Must(d => !d.HasValue || d.Value <= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Birthday must be ≤ today.");
        }
    }
}
