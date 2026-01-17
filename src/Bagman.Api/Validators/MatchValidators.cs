using Bagman.Contracts.Models.Tables;
using FluentValidation;

namespace Bagman.Api.Validators;

public class CreateMatchRequestValidator : AbstractValidator<CreateMatchRequest>
{
    public CreateMatchRequestValidator()
    {
        RuleFor(x => x.Country1)
            .NotEmpty()
            .WithMessage("Pierwsza drużyna jest wymagana")
            .MaximumLength(100)
            .WithMessage("Nazwa drużyny nie może mieć więcej niż 100 znaków");

        RuleFor(x => x.Country2)
            .NotEmpty()
            .WithMessage("Druga drużyna jest wymagana")
            .MaximumLength(100)
            .WithMessage("Nazwa drużyny nie może mieć więcej niż 100 znaków");

        RuleFor(x => x.MatchDateTime)
            .NotEmpty()
            .WithMessage("Data meczu jest wymagana")
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Data meczu musi być w przyszłości");

        RuleFor(x => x)
            .Must(x => x.Country1 != x.Country2)
            .WithMessage("Drużyny muszą być różne");
    }
}

public class UpdateMatchRequestValidator : AbstractValidator<UpdateMatchRequest>
{
    public UpdateMatchRequestValidator()
    {
        RuleFor(x => x.Country1)
            .NotEmpty()
            .WithMessage("Pierwsza drużyna jest wymagana")
            .MaximumLength(100)
            .WithMessage("Nazwa drużyny nie może mieć więcej niż 100 znaków");

        RuleFor(x => x.Country2)
            .NotEmpty()
            .WithMessage("Druga drużyna jest wymagana")
            .MaximumLength(100)
            .WithMessage("Nazwa drużyny nie może mieć więcej niż 100 znaków");

        RuleFor(x => x.MatchDateTime)
            .NotEmpty()
            .WithMessage("Data meczu jest wymagana");

        RuleFor(x => x)
            .Must(x => x.Country1 != x.Country2)
            .WithMessage("Drużyny muszą być różne");
    }
}

public class SetMatchResultRequestValidator : AbstractValidator<SetMatchResultRequest>
{
    public SetMatchResultRequestValidator()
    {
        RuleFor(x => x.Score1)
            .NotEmpty()
            .WithMessage("Wynik pierwszej drużyny jest wymagany")
            .Matches(@"^\d+$")
            .WithMessage("Wynik musi być liczbą");

        RuleFor(x => x.Score2)
            .NotEmpty()
            .WithMessage("Wynik drugiej drużyny jest wymagany")
            .Matches(@"^\d+$")
            .WithMessage("Wynik musi być liczbą");
    }
}

public class PlaceBetRequestValidator : AbstractValidator<PlaceBetRequest>
{
    public PlaceBetRequestValidator()
    {
        RuleFor(x => x.Prediction)
            .NotEmpty()
            .WithMessage("Typowanie jest wymagane")
            .MaximumLength(10)
            .WithMessage("Typowanie nie może mieć więcej niż 10 znaków")
            .Matches(@"^\d+:\d+$|^[XxX]$")
            .WithMessage("Typowanie musi być w formacie 'wynik1:wynik2' lub 'X' (remis)");
    }
}
