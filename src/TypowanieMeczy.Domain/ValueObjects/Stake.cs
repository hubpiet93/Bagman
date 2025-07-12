namespace TypowanieMeczy.Domain.ValueObjects;

public record Stake
{
    public decimal Value { get; }

    public Stake(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Stake cannot be negative", nameof(value));
        
        if (value > 10000)
            throw new ArgumentException("Stake cannot exceed 10000", nameof(value));

        Value = Math.Round(value, 2);
    }

    public static implicit operator decimal(Stake stake) => stake.Value;

    public override string ToString() => Value.ToString("F2");
} 