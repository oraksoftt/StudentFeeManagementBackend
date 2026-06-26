using FluentValidation;
using StudentFee.Core.DTOs;

namespace StudentFee.API.Validators;


public class CreateFeeValidator : AbstractValidator<CreateFeeDto>
{
    public CreateFeeValidator()
    {
        RuleFor(x => x.StudentId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Student is required.");

        RuleFor(x => x.Amount)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Amount is required.")
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.PaymentDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Payment date is required.");
    }
}
