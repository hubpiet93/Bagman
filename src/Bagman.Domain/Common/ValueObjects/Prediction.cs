using System.Text.RegularExpressions;
using ErrorOr;

namespace Bagman.Domain.Common.ValueObjects;

/// <summary>
///     Value object representing a bet prediction (e.g., "2:1", "0:0", "X")
/// </summary>
public sealed record Prediction
{
    private static readonly Regex PredictionPattern = new(@"^\d+:\d+$|^X$", RegexOptions.Compiled);

    public string Value { get; }

    private Prediction(string value)
    {
        Value = value;
    }

    public static ErrorOr<Prediction> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(
                "Prediction.Empty",
                "Prediction cannot be empty");

        if (!PredictionPattern.IsMatch(value))
            return Error.Validation(
                "Prediction.InvalidFormat",
                "Prediction must be in format '2:1' or 'X'");

        return new Prediction(value);
    }

    public static implicit operator string(Prediction prediction)
    {
        return prediction.Value;
    }
}
