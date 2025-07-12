namespace TypowanieMeczy.Domain.ValueObjects;

public record MatchDateTime
{
    public DateTime Value { get; }

    public MatchDateTime(DateTime value)
    {
        if (value <= DateTime.UtcNow)
            throw new ArgumentException("Match date time must be in the future", nameof(value));

        Value = value;
    }

    public static implicit operator DateTime(MatchDateTime matchDateTime) => matchDateTime.Value;

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm:ss");
} 