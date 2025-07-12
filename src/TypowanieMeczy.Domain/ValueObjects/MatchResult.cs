using System.Text.RegularExpressions;

namespace TypowanieMeczy.Domain.ValueObjects;

public record MatchResult
{
    public string Value { get; }

    public MatchResult(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Match result cannot be empty", nameof(value));

        // Validate format: "X:Y" where X and Y are numbers
        if (!Regex.IsMatch(value, @"^\d+:\d+$"))
            throw new ArgumentException("Match result must be in format 'X:Y' (e.g., '2:1')", nameof(value));

        Value = value;
    }

    public static implicit operator string(MatchResult matchResult) => matchResult.Value;

    public override string ToString() => Value;
} 