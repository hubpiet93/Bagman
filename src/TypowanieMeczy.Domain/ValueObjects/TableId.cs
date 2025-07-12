using System.ComponentModel;
using System.Globalization;

namespace TypowanieMeczy.Domain.ValueObjects;

[TypeConverter(typeof(TableIdTypeConverter))]
public record TableId
{
    public Guid Value { get; }

    public TableId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Table ID cannot be empty", nameof(value));
        
        Value = value;
    }

    public static TableId New() => new TableId(Guid.NewGuid());

    public static TableId FromString(string value)
    {
        if (Guid.TryParse(value, out var guid))
            return new TableId(guid);
        
        throw new ArgumentException("Invalid Table ID format", nameof(value));
    }

    public static implicit operator Guid(TableId tableId) => tableId.Value;
    public static implicit operator string(TableId tableId) => tableId.Value.ToString();

    public override string ToString() => Value.ToString();
}

public class TableIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            return TableId.FromString(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }
} 