using Bagman.Contracts.Models.Auth;
using FluentValidation;

namespace Bagman.Api.Validators;

/// <summary>
///     Walidator dla RegisterRequest - rejestracja nowego użytkownika
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login jest wymagany")
            .Length(3, 50).WithMessage("Login musi mieć od 3 do 50 znaków")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Login może zawierać tylko litery, cyfry i podkreślnik")
            .Must(login => login != null && !login.Contains(' ')).WithMessage("Login nie może zawierać spacji");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidłowy format email")
            .MaximumLength(255).WithMessage("Email nie może być dłuższy niż 255 znaków");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane")
            .MinimumLength(10).WithMessage("Hasło musi mieć minimum 10 znaków")
            .MaximumLength(128).WithMessage("Hasło nie może być dłuższe niż 128 znaków")
            .Matches("[A-Z]").WithMessage("Hasło musi zawierać wielką literę")
            .Matches("[a-z]").WithMessage("Hasło musi zawierać małą literę")
            .Matches("[0-9]").WithMessage("Hasło musi zawierać cyfrę")
            .Matches("[^a-zA-Z0-9]").WithMessage("Hasło musi zawierać znak specjalny")
            .Must(password => password != null && !password.Contains(' ')).WithMessage("Hasło nie może zawierać spacji");
    }
}
