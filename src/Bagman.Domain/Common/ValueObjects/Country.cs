using ErrorOr;

namespace Bagman.Domain.Common.ValueObjects;

/// <summary>
/// Value object representing a country name
/// </summary>
public sealed record Country
{
    private Country(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static ErrorOr<Country> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation(
                "Country.Empty",
                "Country name cannot be empty");
        }

        if (name.Length > 100)
        {
            return Error.Validation(
                "Country.TooLong",
                "Country name cannot exceed 100 characters");
        }

        return new Country(name.Trim());
    }

    public static implicit operator string(Country country) => country.Name;
}
