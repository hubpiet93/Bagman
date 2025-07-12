namespace TypowanieMeczy.Domain.ValueObjects;

public record MaxPlayers
{
    public int Value { get; }

    public MaxPlayers(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Max players must be greater than 0", nameof(value));
        
        if (value > 100)
            throw new ArgumentException("Max players cannot exceed 100", nameof(value));

        Value = value;
    }

    public static implicit operator int(MaxPlayers maxPlayers) => maxPlayers.Value;

    public override string ToString() => Value.ToString();
} 