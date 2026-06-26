using FluentValidation;
using StudentFee.Core.DTOs;

namespace StudentFee.API.Validators;

public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MinimumLength(8)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}