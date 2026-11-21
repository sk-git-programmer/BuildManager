using BuildManager.Domain.Common;

namespace BuildManager.Domain.UnitTests.Common;

public sealed class ResultOfTValueTests
{
    private static readonly Error _error = Error.NotFound("Test.NotFound", "Test entity was not found.");

    [Fact]
    public void Success_WithValue_ExposesValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void Failure_WithError_ExposesError()
    {
        var result = Result.Failure<int>(_error);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(_error);
    }

    [Fact]
    public void Failure_WithNullError_ThrowsArgumentNullException()
    {
        var act = () => Result.Failure<int>(null!);

        Should.Throw<ArgumentNullException>(act);
    }

    [Fact]
    public void Value_OnFailure_ThrowsInvalidOperationException()
    {
        var result = Result.Failure<int>(_error);

        // Explicit Func<object?>: Func<int> is not convertible to Shouldly's Func<object?> (no variance for value types).
        Func<object?> act = () => result.Value;

        Should.Throw<InvalidOperationException>(act).Message.ShouldContain(_error.Code);
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesSuccess()
    {
        Result<string> result = "value";

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("value");
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure()
    {
        Result<string> result = _error;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(_error);
    }
}
