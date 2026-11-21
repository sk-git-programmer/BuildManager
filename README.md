# BuildManager

[![CI](https://github.com/OWNER/BuildManager/actions/workflows/ci.yml/badge.svg)](https://github.com/OWNER/BuildManager/actions/workflows/ci.yml)

BuildManager helps a private investor run the construction of a single-family house: budget, stages, expenses,
contractors and construction-loan tranches. At any moment it answers: how much was planned, how much was spent,
what is left, which stage is late or over budget, and what the bank needs before the next tranche.

It is also a portfolio project showing how I build production-ready .NET applications: a rich domain model,
Clean Architecture with enforced boundaries, explicit error handling, tests at every level and documented decisions.

> **Status:** Phase 0 (setup) is done. The domain model is next. See the [roadmap](docs/ROADMAP.md).

## Tech stack

| Area | Choice |
|------|--------|
| Platform | .NET 10 (LTS), C# 14, nullable enabled, warnings as errors |
| Architecture | Clean Architecture: Domain, Application, Infrastructure, Api, Contracts, Web |
| API | ASP.NET Core Minimal API, OpenAPI + Scalar, RFC 9457 ProblemDetails |
| UI | Blazor Web App (interactive WebAssembly), MudBlazor |
| Persistence | EF Core, SQL Server |
| Application patterns | CQRS without a mediator library, Result pattern, FluentValidation, Scrutor decorators |
| Local orchestration | .NET Aspire (AppHost, ServiceDefaults, OpenTelemetry dashboard) |
| Testing | xUnit v3 on Microsoft.Testing.Platform, Shouldly, NSubstitute, NetArchTest, WebApplicationFactory |
| CI | GitHub Actions: format check, build, test |

MediatR, AutoMapper and FluentAssertions are intentionally not used ([ADR 0004](docs/adr/0004-no-commercial-licence-libraries.md)).

## Architecture

```
Domain  <-  Application  <-  Infrastructure
                 ^                 ^
                 └──────  Api  ────┘ ──> Contracts <── Web / Web.Client
```

- **Domain** has no dependencies. Aggregates protect their invariants; business failures are returned as `Result`.
- **Application** contains use cases (commands and queries) and the ports that Infrastructure implements.
- **Api** is the composition root; **Web / Web.Client** talk to it over HTTP using **Contracts** only.
- The rules are enforced by architecture tests on both project references and compiled IL
  ([ADR 0007](docs/adr/0007-architecture-tests-on-two-levels.md)).

Details: [ARCHITECTURE.md](docs/ARCHITECTURE.md) · [DOMAIN.md](docs/DOMAIN.md) · [CONVENTIONS.md](docs/CONVENTIONS.md) · [ADRs](docs/adr/)

## Getting started

### Prerequisites

- [.NET SDK 10.0.404](https://dotnet.microsoft.com/download/dotnet/10.0) or a later 10.0.x SDK (pinned in `global.json`)
- SQL Server, any edition, running locally (Docker is not required)
- Visual Studio 2026, or any editor plus the .NET CLI

### Run locally

```powershell
git clone https://github.com/OWNER/BuildManager.git
cd BuildManager

# Trust the HTTPS development certificate (once per machine)
dotnet dev-certs https --trust

# Connection string for your local SQL Server instance, stored in user secrets (never in the repository)
dotnet user-secrets set "ConnectionStrings:buildmanager" "Server=localhost;Database=BuildManager;Trusted_Connection=True;TrustServerCertificate=True" --project aspire/BuildManager.AppHost

# Start the API, the web app and the Aspire dashboard
dotnet run --project aspire/BuildManager.AppHost
```

Open the dashboard using the login URL printed in the console. The `api` and `web` resources should be
**Running** and **Healthy**; logs, traces and metrics of both are visible in the dashboard.

In Visual Studio, set `BuildManager.AppHost` as the startup project and press F5.

Adjust `Server=` to your instance (for example `localhost\SQLEXPRESS`). The database schema will be created by
EF Core migrations once persistence is added (Phase 3).

### Quality checks

The same commands run in CI:

```powershell
dotnet restore BuildManager.slnx
dotnet format BuildManager.slnx --verify-no-changes --no-restore
dotnet build BuildManager.slnx --configuration Release --no-restore
dotnet test --solution BuildManager.slnx --configuration Release --no-build
```

## Repository layout

```
├─ src/        Production code: Domain, Application, Infrastructure, Contracts, Api, Web, Web.Client
├─ aspire/     AppHost (local orchestration) and ServiceDefaults (telemetry, health checks, resilience)
├─ tests/      Unit, integration and architecture tests
├─ docs/       Architecture, domain, conventions, roadmap and ADRs
└─ .github/    CI workflow
```
