using System.ComponentModel.DataAnnotations;

namespace TypowanieMeczy.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        var emailAttribute = new EmailAddressAttribute();
        if (!emailAttribute.IsValid(value))
            throw new ArgumentException("Invalid email format", nameof(value));

        if (value.Length > 255)
            throw new ArgumentException("Email cannot exceed 255 characters", nameof(value));

        Value = value.Trim().ToLowerInvariant();
    }

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
} 