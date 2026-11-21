namespace BuildManager.Domain.Common;

/// <summary>
/// Category of an expected business failure. The API maps it to an HTTP status code (see ARCHITECTURE.md).
/// </summary>
public enum ErrorType
{
    /// <summary>The input is invalid (400).</summary>
    Validation,

    /// <summary>The requested entity does not exist (404).</summary>
    NotFound,

    /// <summary>The operation is not allowed in the current state (409).</summary>
    Conflict,

    /// <summary>The caller is not allowed to perform the operation (403).</summary>
    Forbidden,
}
