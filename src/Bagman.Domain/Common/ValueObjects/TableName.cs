using ErrorOr;

namespace Bagman.Domain.Common.ValueObjects;

/// <summary>
///     Value object representing a table name
/// </summary>
public sealed record TableName
{
    public string Value { get; }
    private TableName(string value)
    {
        Value = value;
    }

    public static ErrorOr<TableName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(
                "TableName.Empty",
                "Table name cannot be empty");

        if (value.Length > 100)
            return Error.Validation(
                "TableName.TooLong",
                "Table name cannot exceed 100 characters");

        return new TableName(value.Trim());
    }

    public static implicit operator string(TableName tableName)
    {
        return tableName.Value;
    }
}
