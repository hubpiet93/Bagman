namespace TypowanieMeczy.Domain.ValueObjects;

public record Login
{
    public string Value { get; }

    public Login(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Login cannot be empty", nameof(value));
        
        if (value.Length < 3)
            throw new ArgumentException("Login must be at least 3 characters long", nameof(value));
        
        if (value.Length > 50)
            throw new ArgumentException("Login cannot exceed 50 characters", nameof(value));
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9_-]+$"))
            throw new ArgumentException("Login can only contain letters, numbers, underscores and hyphens", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(Login login) => login.Value;

    public override string ToString() => Value;
} 