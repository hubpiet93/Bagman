using System.ComponentModel;
using System.Globalization;

namespace TypowanieMeczy.Domain.ValueObjects;

[TypeConverter(typeof(BetIdTypeConverter))]
public record BetId
{
    public Guid Value { get; }

    public BetId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Bet ID cannot be empty", nameof(value));
        
        Value = value;
    }

    public static BetId New() => new BetId(Guid.NewGuid());

    public static BetId FromString(string value)
    {
        if (Guid.TryParse(value, out var guid))
            return new BetId(guid);
        
        throw new ArgumentException("Invalid Bet ID format", nameof(value));
    }

    public static implicit operator Guid(BetId betId) => betId.Value;
    public static implicit operator string(BetId betId) => betId.Value.ToString();

    public override string ToString() => Value.ToString();
}

public class BetIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            return BetId.FromString(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }
} 