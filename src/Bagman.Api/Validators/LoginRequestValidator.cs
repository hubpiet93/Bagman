using Bagman.Contracts.Models.Auth;
using FluentValidation;

namespace Bagman.Api.Validators;

/// <summary>
///     Walidator dla LoginRequest - logowanie użytkownika
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login jest wymagany")
            .MaximumLength(50).WithMessage("Login nie może być dłuższy niż 50 znaków");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane")
            .MaximumLength(128).WithMessage("Hasło nie może być dłuższe niż 128 znaków");
    }
}
