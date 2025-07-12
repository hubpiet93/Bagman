using FluentValidation;
using TypowanieMeczy.Api.Models;

namespace TypowanieMeczy.Api.Validators;

public class CreateTableRequestValidator : AbstractValidator<CreateTableRequest>
{
    public CreateTableRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 50)
            .Matches("^[a-zA-Z0-9\\s-_]+$")
            .WithMessage("Table name must be 3-50 characters long and contain only letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(4, 20)
            .WithMessage("Password must be 4-20 characters long");

        RuleFor(x => x.MaxPlayers)
            .InclusiveBetween(2, 50)
            .WithMessage("Maximum players must be between 2 and 50");

        RuleFor(x => x.Stake)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000)
            .WithMessage("Stake must be between 0.01 and 1000");

        RuleFor(x => x.IsSecretMode)
            .NotNull()
            .WithMessage("IsSecretMode must be specified");
    }
} 