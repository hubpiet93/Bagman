using ErrorOr;

namespace Bagman.Domain.Common.ValueObjects;

/// <summary>
///     Value object representing money amount
/// </summary>
public sealed record Money
{
    public decimal Amount { get; }

    public static Money Zero => new(0);
    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static ErrorOr<Money> Create(decimal amount)
    {
        if (amount < 0)
            return Error.Validation(
                "Money.Negative",
                "Amount cannot be negative");

        return new Money(amount);
    }

    public static implicit operator decimal(Money money)
    {
        return money.Amount;
    }

    public static Money operator +(Money left, Money right)
    {
        return new Money(left.Amount + right.Amount);
    }
    public static Money operator -(Money left, Money right)
    {
        return new Money(left.Amount - right.Amount);
    }
    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier);
    }
    public static Money operator /(Money money, decimal divisor)
    {
        return new Money(money.Amount / divisor);
    }

    public static bool operator >(Money left, Money right)
    {
        return left.Amount > right.Amount;
    }
    public static bool operator <(Money left, Money right)
    {
        return left.Amount < right.Amount;
    }
    public static bool operator >=(Money left, Money right)
    {
        return left.Amount >= right.Amount;
    }
    public static bool operator <=(Money left, Money right)
    {
        return left.Amount <= right.Amount;
    }
}
