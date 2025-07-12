namespace TypowanieMeczy.Domain.ValueObjects;

public record Country
{
    public string Value { get; }

    public Country(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Country cannot be empty", nameof(value));
        
        if (value.Length > 100)
            throw new ArgumentException("Country name cannot exceed 100 characters", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(Country country) => country.Value;

    public override string ToString() => Value;
} 