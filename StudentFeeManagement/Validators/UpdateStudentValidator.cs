//using FluentValidation;
//using StudentFee.Core.DTOs;

//namespace StudentFee.API.Validators
//{
//    public class UpdateStudentValidator : AbstractValidator<UpdateStudentDto>
//    {
//        public UpdateStudentValidator()
//        {
//            RuleFor(x => x.Name)
//            .NotEmpty()
//            .MinimumLength(3)
//            .MaximumLength(100);

//            RuleFor(x => x.Email)
//                .EmailAddress()
//                .When(x => !string.IsNullOrEmpty(x.Email));

//            RuleFor(x => x.Phone)
//                .MinimumLength(8)
//                .MaximumLength(20)
//                .When(x => !string.IsNullOrEmpty(x.Phone));

//        }
//    }
//}
using FluentValidation;
using StudentFee.Core.DTOs;

namespace StudentFee.API.Validators
{
    public class UpdateStudentValidator : AbstractValidator<UpdateStudentDto>
    {
        public UpdateStudentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .MinimumLength(8).WithMessage("Phone number must be at least 8 characters.")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");
        }
    }
}
