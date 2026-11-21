# Architecture

BuildManager helps a private investor manage the construction of a single-family house: budget, stages, expenses, contractors and construction-loan tranches.

## Solution structure

```
BuildManager.slnx
├─ src/
│  ├─ BuildManager.Domain           Entities, value objects, domain events, business rules. No dependencies.
│  ├─ BuildManager.Application      Use cases (commands/queries), handlers, validators, ports (interfaces).
│  ├─ BuildManager.Infrastructure   EF Core (SQL Server), repositories, file storage, external integrations.
│  ├─ BuildManager.Contracts        Request/response DTOs shared by Api and Web.
│  ├─ BuildManager.Api              ASP.NET Core Minimal API: endpoints, auth, ProblemDetails, OpenAPI.
│  ├─ BuildManager.Web              Blazor Web App host (server): static assets, prerendering, later BFF.
│  └─ BuildManager.Web.Client       Blazor WebAssembly components (MudBlazor), typed API clients.
├─ aspire/
│  ├─ BuildManager.AppHost          .NET Aspire orchestration for local development.
│  └─ BuildManager.ServiceDefaults  OpenTelemetry, health checks, resilience, service discovery.
├─ tests/
│  ├─ BuildManager.Domain.UnitTests          (created in Phase 1)
│  ├─ BuildManager.Application.UnitTests     (created in Phase 2)
│  ├─ BuildManager.Api.IntegrationTests      (created in Phase 4)
│  └─ BuildManager.Architecture.Tests
├─ docs/
│  ├─ ARCHITECTURE.md, DOMAIN.md, CONVENTIONS.md, ROADMAP.md
│  └─ adr/
├─ .github/workflows/ci.yml
└─ global.json, Directory.Build.props, Directory.Packages.props, .editorconfig, .gitattributes
```

A test project is created together with its first test: on Microsoft.Testing.Platform a test project with zero tests fails the run.

## Dependency rules

```
Domain  <-  Application  <-  Infrastructure
                 ^                 ^
                 └──────  Api  ────┘ ──> Contracts <── Web / Web.Client
```

- `Domain` references nothing (BCL only, no packages).
- `Application` references `Domain` only. It defines ports such as `IExpenseRepository`, `IUnitOfWork`, `IFileStorage`.
- `Infrastructure` implements those ports. It is the only project that knows EF Core.
- `Api` is the composition root: it wires up Application and Infrastructure, maps Contracts to commands/queries and Results to HTTP responses.
- `Web` / `Web.Client` know only `Contracts` and talk to the API over HTTP. They never reference Application or Domain. `Web` references `Web.Client`, not `Contracts` directly.
- `ServiceDefaults` is referenced only by the server hosts (`Api`, `Web`) and references no project.
- `AppHost` references `Api` and `Web` as orchestrated resources, not as code dependencies; nothing references `AppHost`.
- Rules are enforced by `BuildManager.Architecture.Tests` on two levels ([ADR 0007](adr/0007-architecture-tests-on-two-levels.md)):
  - `.csproj` project references against an allow-list (catches a rule violation as soon as the reference is added);
  - compiled IL with NetArchTest against forbidden namespaces, including `Microsoft.EntityFrameworkCore` and `Microsoft.AspNetCore` in Domain and Application (catches transitive usage).
- The allow-list lives in `tests/BuildManager.Architecture.Tests/DependencyRules.cs`. A new production project fails the tests until it is added there; update this section in the same change.

## Key patterns

**Rich domain model.** Aggregates protect their invariants. State changes go through methods (`stage.Complete(date)`), never through public setters. Strongly-typed IDs (`ExpenseId`, `StageId`) prevent mixing up identifiers.

**Result pattern.** Expected business failures return `Result` / `Result<T>` with an `Error(Code, Description, ErrorType)`. Exceptions are reserved for bugs and infrastructure failures. The API maps `ErrorType` to status codes (Validation → 400, NotFound → 404, Conflict → 409, Forbidden → 403) as RFC 9457 ProblemDetails.

**CQRS without a mediator library.**
```csharp
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken ct);
}
```
Handlers are injected directly into endpoints. Cross-cutting concerns (validation, logging) are decorators registered with Scrutor. Queries may bypass aggregates and project straight to DTOs with `AsNoTracking()`.

**Domain events.** Aggregates raise events (e.g. `StageBudgetExceeded`). They are dispatched after `SaveChanges` succeeds, in the same request. An outbox is out of scope until there is a real need.

**Persistence.** EF Core with `IEntityTypeConfiguration<T>` per entity, value converters for strongly-typed IDs and value objects, owned types for `Money` and `Address`. Migrations live in Infrastructure. The schema is created only via migrations.

**Time.** `TimeProvider` is injected; `DateTime.Now` is not used. Dates without time use `DateOnly`.

## Runtime topology

- Local: Aspire AppHost starts Api and Web; SQL Server is a local instance passed in via `AddConnectionString("buildmanager")`. Connection strings live in AppHost user secrets, never in `appsettings.json`. The AppHost has no container resources, so Docker is not required.
- Each server host calls `AddServiceDefaults()` (OpenTelemetry exported to the Aspire dashboard, health checks `/health` and `/alive` in Development, service discovery, HTTP resilience).
- MVP: Web.Client calls the API directly (CORS limited to the Web origin).
- Later: BFF – the Web host proxies `/api` to the API (YARP) and holds the auth cookie, so no tokens live in the browser.

## Decisions log

Full ADRs live in [`docs/adr/`](adr/). Summary:

| #    | Decision | Why |
|------|----------|-----|
| [0001](adr/0001-dotnet-10-lts.md) | .NET 10 LTS | .NET 8 and 9 reached end of support on 10 Nov 2026. |
| [0002](adr/0002-clean-architecture-with-separate-api-and-web.md) | Clean Architecture with separate Api and Web | Clear boundaries; UI replaceable; showcases layering and testability. |
| [0003](adr/0003-sql-server-local-instance.md) | SQL Server, local instance for development | Standard in the .NET ecosystem; no Docker required yet. Switch to an Aspire container when Docker is available. |
| [0004](adr/0004-no-commercial-licence-libraries.md) | No MediatR / AutoMapper / FluentAssertions | Commercial licences since 2025; own handler interfaces + Scrutor decorators, manual mapping, Shouldly. |
| [0005](adr/0005-result-pattern-for-business-errors.md) | Result pattern for business errors | Explicit failure paths, no exceptions for control flow. |
| [0006](adr/0006-blazor-web-app-interactive-webassembly.md) | Blazor Web App with interactive WebAssembly + MudBlazor | Single C# stack; WASM keeps the API as the only backend contract. |
| [0007](adr/0007-architecture-tests-on-two-levels.md) | Architecture tests on project references and IL | IL alone cannot see unused references or empty assemblies; project files alone cannot see transitive usage. |
