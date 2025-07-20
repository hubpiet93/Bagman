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
            .MaximumLength(1000).WithMessage("Refresh token jest zbyt długi");
    }
}
