using ErrorOr;

namespace Bagman.Domain.Common;

public static class Result
{
    public static ErrorOr<T> Success<T>(T value) => value;
    public static ErrorOr<T> Failure<T>(Error error) => error;
} 