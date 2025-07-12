using System.ComponentModel;
using System.Globalization;

namespace TypowanieMeczy.Domain.ValueObjects;

[TypeConverter(typeof(MatchIdTypeConverter))]
public record MatchId
{
    public Guid Value { get; }

    public MatchId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Match ID cannot be empty", nameof(value));
        
        Value = value;
    }

    public static MatchId New() => new MatchId(Guid.NewGuid());

    public static MatchId FromString(string value)
    {
        if (Guid.TryParse(value, out var guid))
            return new MatchId(guid);
        
        throw new ArgumentException("Invalid Match ID format", nameof(value));
    }

    public static implicit operator Guid(MatchId matchId) => matchId.Value;
    public static implicit operator string(MatchId matchId) => matchId.Value.ToString();

    public override string ToString() => Value.ToString();
}

public class MatchIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            return MatchId.FromString(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }
} 