using FluentValidation;
using LoadBalancer.DAL.DTOs.CalculationDtos;
using LoadBalancer.DAL.Validation.Utils;

namespace LoadBalancer.DAL.Validation
{
    public class TrigonometryRequestDtoValidator : AbstractValidator<TrigonometryRequestDto>
    {
        public TrigonometryRequestDtoValidator()
        {
            RuleFor(dto => dto.N)
                .NotEmpty().WithMessage("N is required")
                .InclusiveBetween(Constants.ACCURACY_MIN_VALUE, Constants.ACCURACY_MAX_VALUE)
                .WithMessage($"N must be [{Constants.ACCURACY_MIN_VALUE}; {Constants.ACCURACY_MAX_VALUE}]");

            RuleFor(dto => dto.XForSin)
                .InclusiveBetween(Constants.X_MIN_VALUE, Constants.X_MAX_VALUE)
                .When(dto => dto.XForSin.HasValue).WithMessage($"XForSin must be [{Constants.X_MIN_VALUE}; {Constants.X_MAX_VALUE}]");

            RuleFor(dto => dto.XForCos)
                .InclusiveBetween(Constants.X_MIN_VALUE, Constants.X_MAX_VALUE)
                .When(dto => dto.XForCos.HasValue).WithMessage($"XForCos must be [{Constants.X_MIN_VALUE}; {Constants.X_MAX_VALUE}]");

            RuleFor(dto => dto.ConnectionId)
                .NotEmpty().WithMessage("ConnectionId is required");
        }
    }
}
