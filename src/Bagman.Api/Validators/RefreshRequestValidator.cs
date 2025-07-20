using Bagman.Contracts.Models.Auth;
using FluentValidation;

namespace Bagman.Api.Validators;

/// <summary>
///     Walidator dla RefreshRequest - odświeżenie tokenu
/// </summary>
public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token jest wymagany")
            .MaximumLength(1000).WithMessage("Refresh token jest zbyt długi")
            .Must(BeValidJwtFormat).WithMessage("Nieprawidłowy format refresh token");
    }

    private static bool BeValidJwtFormat(string? token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        // Sprawdź czy token ma format JWT (3 części oddzielone kropkami)
        var parts = token.Split('.');
        return parts.Length == 3;
    }
}
