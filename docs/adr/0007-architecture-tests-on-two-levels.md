# ADR 0007: Architecture tests on project references and IL

- **Status:** Accepted
- **Date:** 2026-11-19

## Context

The dependency rules from ARCHITECTURE.md must be enforced automatically, starting with an empty solution, and a
test must fail as soon as someone breaks a rule.

NetArchTest analyses compiled IL. This has two blind spots:

1. An assembly without types (e.g. an empty `Domain` in Phase 0) passes every rule trivially.
2. The compiler does not emit references to assemblies whose types are not used. Adding a forbidden
   `<ProjectReference>` is invisible in IL until code starts using it.

Checking only `.csproj` files has the opposite blind spot: a type from a forbidden layer can be used through a
transitive reference (e.g. a package that pulls in EF Core) without any direct reference in the project file.

## Decision

Enforce the rules in `BuildManager.Architecture.Tests` on two levels, from one source of truth
(`DependencyRules.cs`):

1. **Project references.** Every production `.csproj` under `src/` and `aspire/` is parsed as XML. Its
   `ProjectReference` items must be a subset of an allow-list per project. Additional checks: `Domain` has no
   `PackageReference` or `FrameworkReference`, and the allow-list must list exactly the production projects found in
   the repository (a new project fails the tests until it is classified).
2. **IL (NetArchTest).** For each assembly, types must not depend on forbidden namespaces – other layers, and
   `Microsoft.EntityFrameworkCore` / `Microsoft.AspNetCore` for `Domain` and `Application`. Each
   (assembly, namespace) pair is a separate theory case so a failure points at the exact rule.

Assemblies are loaded by name (`Assembly.Load`) instead of marker types, so no production code exists only for tests
and the `Program` classes of `Api` and `Web` do not conflict.

## Consequences

- A rule violation is detected as soon as the reference is added, even in an empty solution; transitive usage is
  detected once code uses the forbidden type.
- Adding a production project requires updating `DependencyRules.cs` and ARCHITECTURE.md in the same change.
- Project paths in `.csproj` use `\`; the parser normalises them so the tests pass on Linux CI.
- `NetArchTest.Rules` is no longer actively developed. If it stops working on a future .NET version, the IL level can
  move to the `NetArchTest.eNhancedEdition` fork or ArchUnitNET without changing the project-reference level.
- Namespace rules match by prefix: `BuildManager.Web` also covers `BuildManager.Web.Client`, which is why the
  Web.Client rule does not list `BuildManager.Web`.
