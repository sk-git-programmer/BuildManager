using BuildManager.Domain.Common;

namespace BuildManager.Domain.UnitTests.Common;

public sealed class ErrorTests
{
    private const string Code = "Test.Reason";
    private const string Description = "Test description.";

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithMissingCode_ThrowsArgumentException(string code)
    {
        var act = () => new Error(code, Description, ErrorType.Validation);

        Should.Throw<ArgumentException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithMissingDescription_ThrowsArgumentException(string description)
    {
        var act = () => new Error(Code, description, ErrorType.Validation);

        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithUndefinedType_ThrowsArgumentOutOfRangeException()
    {
        var act = () => new Error(Code, Description, (ErrorType)999);

        Should.Throw<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Validation_WithCodeAndDescription_CreatesValidationError()
    {
        var error = Error.Validation(Code, Description);

        error.Type.ShouldBe(ErrorType.Validation);
        error.Code.ShouldBe(Code);
        error.Description.ShouldBe(Description);
    }

    [Fact]
    public void NotFound_WithCodeAndDescription_CreatesNotFoundError()
    {
        var error = Error.NotFound(Code, Description);

        error.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public void Conflict_WithCodeAndDescription_CreatesConflictError()
    {
        var error = Error.Conflict(Code, Description);

        error.Type.ShouldBe(ErrorType.Conflict);
    }

    [Fact]
    public void Forbidden_WithCodeAndDescription_CreatesForbiddenError()
    {
        var error = Error.Forbidden(Code, Description);

        error.Type.ShouldBe(ErrorType.Forbidden);
    }

    [Fact]
    public void Equals_WithSameValues_ReturnsTrue()
    {
        var first = Error.Conflict(Code, Description);
        var second = Error.Conflict(Code, Description);

        var areEqual = first.Equals(second);

        areEqual.ShouldBeTrue();
    }
}
