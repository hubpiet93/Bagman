using FluentValidation;
using TypowanieMeczy.Api.Models;

namespace TypowanieMeczy.Api.Validators;

public class CreateMatchRequestValidator : AbstractValidator<CreateMatchRequest>
{
    public CreateMatchRequestValidator()
    {
        RuleFor(x => x.TableId)
            .NotEmpty()
            .WithMessage("Table ID is required");

        RuleFor(x => x.Country1)
            .NotEmpty()
            .Length(2, 3)
            .Matches("^[A-Z]{2,3}$")
            .WithMessage("Country1 must be a 2-3 letter country code in uppercase");

        RuleFor(x => x.Country2)
            .NotEmpty()
            .Length(2, 3)
            .Matches("^[A-Z]{2,3}$")
            .WithMessage("Country2 must be a 2-3 letter country code in uppercase");

        RuleFor(x => x.MatchDateTime)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow.AddMinutes(30))
            .WithMessage("Match date must be at least 30 minutes in the future");

        RuleFor(x => x)
            .Must(x => x.Country1 != x.Country2)
            .WithMessage("Country1 and Country2 must be different");
    }
} 