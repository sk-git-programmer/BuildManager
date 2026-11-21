# Roadmap

Status: ⬜ not started · 🟨 in progress · ✅ done

This file in the repository (`docs/ROADMAP.md`) is the source of truth. Update it in the pull request that finishes a feature, then refresh the copy in Project knowledge.

## Phase 0 – Setup ✅
- ✅ .NET 10 SDK, Visual Studio 2026, SQL Server + SSMS
- ✅ Solution skeleton: `BuildManager.slnx`, `src/` projects with references following the dependency rules, minimal Api and Blazor Web App (interactive WebAssembly)
- ✅ `Directory.Build.props`, Central Package Management, `.editorconfig`, `.gitattributes` (LF), `global.json` (SDK 10.0.4xx, Aspire SDK, Microsoft.Testing.Platform)
- ✅ Aspire AppHost + ServiceDefaults: Api and Web orchestrated, local SQL Server via `AddConnectionString("buildmanager")` and user secrets, no Docker
- ✅ Architecture tests for dependency rules on project references and IL ([ADR 0007](adr/0007-architecture-tests-on-two-levels.md))
- ✅ GitHub repo + CI (format check, Release build with warnings as errors, tests), protected `main`
- ✅ README, documentation in `docs/`, ADRs 0001–0007

## Phase 1 – Domain core ⬜
- ⬜ `BuildManager.Domain.UnitTests` project (created with the first test)
- ⬜ Building blocks: `Entity`, `AggregateRoot` (domain events), `Result`, `Error`, strongly-typed IDs
- ⬜ Value objects: `Money`, `Nip`, `DateRange`, `Address`, `VatRate`
- ⬜ `ConstructionProject` with `Stage` and `BudgetItem` + invariants
- ⬜ `Expense` with payment and funding source
- ⬜ `Contractor`
- ⬜ Unit tests for all invariants

Done when: all rules from DOMAIN.md are covered by tests; Domain has no dependencies.

## Phase 2 – Application ⬜
- ⬜ `BuildManager.Application.UnitTests` project (created with the first test)
- ⬜ Abstractions: `ICommand`, `IQuery`, handlers, `IUnitOfWork`, repository ports
- ⬜ Decorators: validation (FluentValidation), logging
- ⬜ Use cases: create project, add/reorder/complete stage, add budget item, record expense, mark expense paid, add contractor
- ⬜ Unit tests for handlers

## Phase 3 – Persistence ⬜
- ⬜ `BuildManagerDbContext`, entity configurations, value converters
- ⬜ Repositories, unit of work, domain event dispatch after save
- ⬜ Initial migration, development seed data
- ⬜ Query projections: budget overview, expense list

## Phase 4 – API ⬜
- ⬜ Endpoint discovery (`IEndpoint`), Result → ProblemDetails mapping
- ⬜ Endpoints for Phase 2 use cases and queries
- ⬜ OpenAPI + Scalar UI
- ⬜ `BuildManager.Api.IntegrationTests` on `BuildManager_Tests` database; CI job with a SQL Server service container
- ⬜ CORS limited to the Web origin; API base URL passed to Web.Client (Aspire wiring of Api, Web and the connection string is done in Phase 0)

## Phase 5 – Web UI (MVP) ⬜
- ⬜ Layout with MudBlazor, typed API client from Contracts
- ⬜ Prerendering strategy for pages that load data (register API clients in Web or disable prerendering) + ADR
- ⬜ Projects list and project details with stages
- ⬜ Expense form and list with filters
- ⬜ Budget overview dashboard (planned vs spent per stage, over-budget warnings)

🎯 **MVP milestone** – usable for my own house build.

## Phase 6 – Authentication ⬜
- ⬜ ASP.NET Core Identity, per-user data isolation
- ⬜ BFF: Web host proxies the API (YARP), cookie auth

## Phase 7 – Documents ⬜
- ⬜ Upload invoices/receipts to expenses (`IFileStorage`, local disk → Azure Blob)

## Phase 8 – Construction loan ⬜
- ⬜ `ConstructionLoan` with tranches, tranche requests tied to completed stages
- ⬜ Funding view: own funds vs loan, next tranche

## Phase 9 – AI invoice scanning ⬜
- ⬜ Extract amount, date, seller and NIP from a photo/PDF of an invoice; user confirms before saving

## Phase 10 – Deployment ⬜
- ⬜ Containers, Aspire deployment to Azure Container Apps (or alternative); revisit Aspire CLI bundle (ASPIRE010)
- ⬜ Production hardening: health endpoints exposure, HSTS settings
- ⬜ Public demo with sample data

## Current focus
Phase 1 – Domain core.
