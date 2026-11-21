# ADR 0003: SQL Server, local instance for development

- **Status:** Accepted
- **Date:** 2026-11-17

## Context

The data is relational (projects, stages, budget items, expenses, contractors) and needs transactions and
reporting queries. SQL Server is the standard choice in the .NET ecosystem and is supported first-class by EF Core
and Aspire. Docker is not available on the development machine yet, so a SQL Server container managed by Aspire
cannot be used for now.

## Decision

- Use SQL Server with EF Core.
- In development, use a locally installed SQL Server instance. The AppHost exposes it as
  `builder.AddConnectionString("buildmanager")`; the value is stored in AppHost user secrets
  (`ConnectionStrings:buildmanager`) and injected into the Api by `WithReference`.
- Connection strings are never stored in `appsettings.json` or committed to the repository.
- Integration tests use a dedicated database, `BuildManager_Tests`.

## Consequences

- Every developer needs a local SQL Server instance and must set the user secret once (documented in README).
- When Docker becomes available, switching to an Aspire-managed container is a change in the AppHost only
  (`AddSqlServer("sql").AddDatabase("buildmanager")`); the resource name stays the same, so the Api is unchanged.
- CI does not need SQL Server until integration tests exist (Phase 4); then it uses a SQL Server service container.
