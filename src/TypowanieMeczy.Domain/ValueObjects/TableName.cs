namespace TypowanieMeczy.Domain.ValueObjects;

public record TableName
{
    public string Value { get; }

    public TableName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Table name cannot be empty", nameof(value));
        
        if (value.Length < 3)
            throw new ArgumentException("Table name must be at least 3 characters long", nameof(value));
        
        if (value.Length > 100)
            throw new ArgumentException("Table name cannot exceed 100 characters", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(TableName tableName) => tableName.Value;

    public override string ToString() => Value;
} 