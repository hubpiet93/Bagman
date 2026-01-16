using Bagman.Contracts.Models.Auth;
using FluentValidation;

namespace Bagman.Api.Validators;

/// <summary>
///     Walidator dla LogoutRequest - wylogowanie użytkownika
/// </summary>
public class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token jest wymagany")
            .MaximumLength(1000).WithMessage("Refresh token jest zbyt długi");
    }
}
