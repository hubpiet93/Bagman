using FluentValidation;
using TypowanieMeczy.Api.Models;

namespace TypowanieMeczy.Api.Validators;

public class PlaceBetRequestValidator : AbstractValidator<PlaceBetRequest>
{
    public PlaceBetRequestValidator()
    {
        RuleFor(x => x.Prediction)
            .NotEmpty()
            .Matches("^[0-9]+:[0-9]+$")
            .WithMessage("Prediction must be in format 'X:Y' where X and Y are numbers (e.g., '2:1', '0:0')");

        RuleFor(x => x.Prediction)
            .Must(BeValidScore)
            .WithMessage("Prediction must be a valid football score (reasonable numbers)");
    }

    private bool BeValidScore(string prediction)
    {
        if (string.IsNullOrEmpty(prediction)) return false;

        var parts = prediction.Split(':');
        if (parts.Length != 2) return false;

        if (!int.TryParse(parts[0], out var homeScore) || !int.TryParse(parts[1], out var awayScore))
            return false;

        // Reasonable football score limits
        return homeScore >= 0 && homeScore <= 20 && awayScore >= 0 && awayScore <= 20;
    }
} 