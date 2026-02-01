using ErrorOr;

namespace Bagman.Domain.Common.ValueObjects;

/// <summary>
/// Value object representing money amount
/// </summary>
public sealed record Money
{
    private Money(decimal amount)
    {
        Amount = amount;
    }

    public decimal Amount { get; }

    public static ErrorOr<Money> Create(decimal amount)
    {
        if (amount < 0)
        {
            return Error.Validation(
                "Money.Negative",
                "Amount cannot be negative");
        }

        return new Money(amount);
    }

    public static Money Zero => new(0);

    public static implicit operator decimal(Money money) => money.Amount;

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);
    public static Money operator -(Money left, Money right) => new(left.Amount - right.Amount);
    public static Money operator *(Money money, decimal multiplier) => new(money.Amount * multiplier);
    public static Money operator /(Money money, decimal divisor) => new(money.Amount / divisor);
    
    public static bool operator >(Money left, Money right) => left.Amount > right.Amount;
    public static bool operator <(Money left, Money right) => left.Amount < right.Amount;
    public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;
    public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;
}
