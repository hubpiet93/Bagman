using System.ComponentModel;
using System.Globalization;

namespace TypowanieMeczy.Domain.ValueObjects;

[TypeConverter(typeof(PoolIdTypeConverter))]
public record PoolId
{
    public Guid Value { get; }

    public PoolId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Pool ID cannot be empty", nameof(value));
        
        Value = value;
    }

    public static PoolId New() => new PoolId(Guid.NewGuid());

    public static PoolId FromString(string value)
    {
        if (Guid.TryParse(value, out var guid))
            return new PoolId(guid);
        
        throw new ArgumentException("Invalid Pool ID format", nameof(value));
    }

    public static implicit operator Guid(PoolId poolId) => poolId.Value;
    public static implicit operator string(PoolId poolId) => poolId.Value.ToString();

    public override string ToString() => Value.ToString();
}

public class PoolIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            return PoolId.FromString(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }
} 