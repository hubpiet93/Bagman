using System.ComponentModel;
using System.Globalization;

namespace TypowanieMeczy.Domain.ValueObjects;

[TypeConverter(typeof(UserIdTypeConverter))]
public record UserId
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(value));
        
        Value = value;
    }

    public static UserId New() => new UserId(Guid.NewGuid());

    public static UserId FromString(string value)
    {
        if (Guid.TryParse(value, out var guid))
            return new UserId(guid);
        
        throw new ArgumentException("Invalid User ID format", nameof(value));
    }

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator string(UserId userId) => userId.Value.ToString();

    public override string ToString() => Value.ToString();
}

public class UserIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            return UserId.FromString(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }
} 