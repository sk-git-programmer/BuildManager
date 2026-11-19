namespace BuildManager.Architecture.Tests;

/// <summary>
/// Executable form of the dependency rules described in docs/ARCHITECTURE.md.
/// Any change here must be reflected there, and vice versa.
/// </summary>
internal static class DependencyRules
{
    private const string AspNetCore = "Microsoft.AspNetCore";
    private const string EntityFrameworkCore = "Microsoft.EntityFrameworkCore";

    /// <summary>
    /// Direct project references each production project is allowed to have.
    /// Every production project must be listed, even with no allowed references.
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> AllowedProjectReferences { get; } =
        new Dictionary<string, string[]>
        {
            [ProjectNames.Domain] = [],
            [ProjectNames.Application] = [ProjectNames.Domain],
            [ProjectNames.Infrastructure] = [ProjectNames.Application, ProjectNames.Domain],
            [ProjectNames.Contracts] = [],
            [ProjectNames.Api] =
            [
                ProjectNames.Application,
                ProjectNames.Infrastructure,
                ProjectNames.Contracts,
                ProjectNames.ServiceDefaults,
            ],
            [ProjectNames.WebClient] = [ProjectNames.Contracts],
            [ProjectNames.Web] = [ProjectNames.WebClient, ProjectNames.ServiceDefaults],
            [ProjectNames.ServiceDefaults] = [],
            [ProjectNames.AppHost] = [ProjectNames.Api, ProjectNames.Web],
        };

    /// <summary>
    /// Namespaces the types of each assembly must not depend on (checked on IL, including transitive usage).
    /// Matching is by prefix, so "BuildManager.Web" also covers "BuildManager.Web.Client".
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> ForbiddenNamespaces { get; } =
        new Dictionary<string, string[]>
        {
            [ProjectNames.Domain] =
            [
                ProjectNames.Application,
                ProjectNames.Infrastructure,
                ProjectNames.Contracts,
                ProjectNames.Api,
                ProjectNames.Web,
                ProjectNames.ServiceDefaults,
                AspNetCore,
                EntityFrameworkCore,
            ],
            [ProjectNames.Application] =
            [
                ProjectNames.Infrastructure,
                ProjectNames.Contracts,
                ProjectNames.Api,
                ProjectNames.Web,
                ProjectNames.ServiceDefaults,
                AspNetCore,
                EntityFrameworkCore,
            ],
            [ProjectNames.Infrastructure] =
            [
                ProjectNames.Contracts,
                ProjectNames.Api,
                ProjectNames.Web,
                ProjectNames.ServiceDefaults,
            ],
            [ProjectNames.Contracts] =
            [
                ProjectNames.Domain,
                ProjectNames.Application,
                ProjectNames.Infrastructure,
                ProjectNames.Api,
                ProjectNames.Web,
                ProjectNames.ServiceDefaults,
            ],
            [ProjectNames.WebClient] =
            [
                ProjectNames.Domain,
                ProjectNames.Application,
                ProjectNames.Infrastructure,
                ProjectNames.Api,
                ProjectNames.ServiceDefaults,
            ],
            [ProjectNames.Web] =
            [
                ProjectNames.Domain,
                ProjectNames.Application,
                ProjectNames.Infrastructure,
                ProjectNames.Api,
            ],
        };
}
