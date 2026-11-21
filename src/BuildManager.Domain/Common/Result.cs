using System.Diagnostics.CodeAnalysis;

namespace BuildManager.Domain.Common;

/// <summary>
/// Outcome of an operation that can fail for an expected business reason (ADR 0005).
/// </summary>
public class Result
{
    private static readonly Result _success = new(error: null);

    private protected Result(Error? error)
    {
        Error = error;
    }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => Error is not null;

    /// <summary>The failure reason; <see langword="null"/> when the result is a success.</summary>
    public Error? Error { get; }

    public static Result Success() => _success;

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result(error);
    }

    public static Result<TValue> Success<TValue>(TValue value) => new(value);

    public static Result<TValue> Failure<TValue>(Error error) => new(error);

    public static implicit operator Result(Error error) => Failure(error);
}
