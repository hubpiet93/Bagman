using System.Text.RegularExpressions;
using ErrorOr;

namespace Bagman.Domain.Common.ValueObjects;

/// <summary>
///     Value object representing a match result score (e.g., "2:1", "0:0")
/// </summary>
public sealed record Score
{
    private static readonly Regex ScorePattern = new(@"^\d+:\d+$", RegexOptions.Compiled);

    public string Value { get; }

    private Score(string value)
    {
        Value = value;
    }

    public static ErrorOr<Score> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(
                "Score.Empty",
                "Score cannot be empty");

        if (!ScorePattern.IsMatch(value))
            return Error.Validation(
                "Score.InvalidFormat",
                "Score must be in format '2:1'");

        return new Score(value);
    }

    public static implicit operator string(Score score)
    {
        return score.Value;
    }
}
