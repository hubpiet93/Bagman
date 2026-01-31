using Bagman.Contracts.Models.Tables;
using FluentValidation;

namespace Bagman.Api.Validators;

public class CreateTableRequestValidator : AbstractValidator<CreateTableRequest>
{
    public CreateTableRequestValidator()
    {
        RuleFor(x => x.UserLogin)
            .NotEmpty()
            .WithMessage("Login użytkownika jest wymagany")
            .MinimumLength(3)
            .WithMessage("Login musi mieć co najmniej 3 znaki")
            .MaximumLength(50)
            .WithMessage("Login nie może mieć więcej niż 50 znaków");

        RuleFor(x => x.UserPassword)
            .NotEmpty()
            .WithMessage("Hasło użytkownika jest wymagane")
            .MinimumLength(10)
            .WithMessage("Hasło musi mieć co najmniej 10 znaków");

        RuleFor(x => x.TableName)
            .NotEmpty()
            .WithMessage("Nazwa stołu jest wymagana")
            .MaximumLength(100)
            .WithMessage("Nazwa stołu nie może mieć więcej niż 100 znaków");

        RuleFor(x => x.TablePassword)
            .NotEmpty()
            .WithMessage("Hasło stołu jest wymagane")
            .MaximumLength(255)
            .WithMessage("Hasło nie może mieć więcej niż 255 znaków");

        RuleFor(x => x.MaxPlayers)
            .GreaterThan(0)
            .WithMessage("Maksymalna liczba graczy musi być większa niż 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Maksymalna liczba graczy nie może przekraczać 1000");

        RuleFor(x => x.Stake)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stawka nie może być ujemna");
    }
}

public class AuthorizedCreateTableRequestValidator : AbstractValidator<AuthorizedCreateTableRequest>
{
    public AuthorizedCreateTableRequestValidator()
    {
        RuleFor(x => x.TableName)
            .NotEmpty()
            .WithMessage("Nazwa stołu jest wymagana")
            .MaximumLength(100)
            .WithMessage("Nazwa stołu nie może mieć więcej niż 100 znaków");

        RuleFor(x => x.TablePassword)
            .NotEmpty()
            .WithMessage("Hasło stołu jest wymagane")
            .MaximumLength(255)
            .WithMessage("Hasło nie może mieć więcej niż 255 znaków");

        RuleFor(x => x.MaxPlayers)
            .GreaterThan(0)
            .WithMessage("Maksymalna liczba graczy musi być większa niż 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Maksymalna liczba graczy nie może przekraczać 1000");

        RuleFor(x => x.Stake)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stawka nie może być ujemna");
    }
}

public class JoinTableRequestValidator : AbstractValidator<JoinTableRequest>
{
    public JoinTableRequestValidator()
    {
        RuleFor(x => x.UserLogin)
            .NotEmpty()
            .WithMessage("Login użytkownika jest wymagany");

        RuleFor(x => x.UserPassword)
            .NotEmpty()
            .WithMessage("Hasło użytkownika jest wymagane");

        RuleFor(x => x.TableName)
            .NotEmpty()
            .WithMessage("Nazwa stołu jest wymagana");

        RuleFor(x => x.TablePassword)
            .NotEmpty()
            .WithMessage("Hasło stołu jest wymagane");
    }
}

public class GrantAdminRequestValidator : AbstractValidator<GrantAdminRequest>
{
    public GrantAdminRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID użytkownika jest wymagane");
    }
}
