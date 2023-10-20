using FluentValidation;
using LoadBalancer.DAL.DTOs.AuthDtos;
using LoadBalancer.DAL.Validation.Utils;

namespace LoadBalancer.DAL.Validation
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(Constants.EMAIL_MAX_LENGTH).WithMessage($"Email must not exceed {Constants.EMAIL_MAX_LENGTH} characters")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required")
                .MaximumLength(Constants.PASSWORD_MAX_LENGTH).WithMessage($"Password must not exceed {Constants.PASSWORD_MAX_LENGTH} characters");
        }
    }
}
