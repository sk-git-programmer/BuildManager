namespace BuildManager.Domain.Common;

/// <summary>
/// Outcome of an operation that returns a value on success (ADR 0005).
/// </summary>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue value)
        : base(error: null)
    {
        _value = value;
    }

    internal Result(Error error)
        : base(error ?? throw new ArgumentNullException(nameof(error)))
    {
    }

    /// <summary>
    /// The value of a successful result. Accessing it on a failure is a programming error.
    /// </summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException(
            $"The value of a failed result cannot be accessed. Error: {Error.Code}.");

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(Error error) => new(error);
}
