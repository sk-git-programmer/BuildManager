using BuildManager.Domain.Common;

namespace BuildManager.Domain.UnitTests.Common;

public sealed class ResultTests
{
    private static readonly Error _error = Error.Conflict("Test.Conflict", "Test conflict.");

    [Fact]
    public void Success_WhenCreated_HasNoError()
    {
        var result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void Failure_WithError_ExposesError()
    {
        var result = Result.Failure(_error);

        result.IsFailure.ShouldBeTrue();
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(_error);
    }

    [Fact]
    public void Failure_WithNullError_ThrowsArgumentNullException()
    {
        var act = () => Result.Failure(null!);

        Should.Throw<ArgumentNullException>(act);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure()
    {
        Result result = _error;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(_error);
    }
}
