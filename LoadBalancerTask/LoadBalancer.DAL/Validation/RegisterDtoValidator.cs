using FluentValidation;
using LoadBalancer.DAL.DTOs.AuthDtos;
using LoadBalancer.DAL.Validation.Utils;

namespace LoadBalancer.DAL.Validation
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(Constants.USERNAME_MAX_LENGTH).WithMessage($"Username must not exceed {Constants.USERNAME_MAX_LENGTH} characters");

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
