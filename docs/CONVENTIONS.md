# Conventions

## General
- Language for code, comments, commits and docs: English.
- `global.json` pins the SDK to the .NET 10 line (`10.0.404`, `rollForward: latestFeature`, no prerelease), the Aspire AppHost SDK version (`msbuild-sdks`) and the test runner (Microsoft.Testing.Platform).
- `Directory.Build.props`: `net10.0`, `Nullable`, `ImplicitUsings`, `TreatWarningsAsErrors`, `MSBuildTreatWarningsAsErrors`, `AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild`, `GenerateDocumentationFile` (required for IDE0005 in build; CS1591 suppressed), NuGet audit of direct and transitive packages.
- Central Package Management (`Directory.Packages.props`) – no versions in `.csproj`. Transitive pinning is enabled to fix vulnerable transitive packages. Microsoft.Extensions.* and ASP.NET Core packages stay on the 10.x line.
- Warnings are not suppressed "just in case". Every suppression is scoped as narrowly as possible (`.editorconfig` section or a single project) and has a comment explaining why.
- Style enforced by `.editorconfig`; rules with severity `warning` fail the build and the CI format check, `suggestion` is IDE guidance only.
- Line endings: LF everywhere (`.gitattributes` `* text=auto eol=lf` + `end_of_line = lf`). Run `dotnet format whitespace BuildManager.slnx` if an editor introduces CRLF.
- File-scoped namespaces; namespace matches the folder; primary constructors where they improve readability; `sealed` by default for classes not designed for inheritance.
- Every async method takes a `CancellationToken` and has no `Async` suffix in handlers/endpoints (suffix is fine in repository/infrastructure APIs).
- `TimeProvider` for current time; `DateOnly` for dates without time.

## Folder structure (feature folders)

```
Domain/
  Projects/        ConstructionProject.cs, Stage.cs, BudgetItem.cs, ProjectErrors.cs, Events/
  Expenses/
  Contractors/
  Common/          Money.cs, Nip.cs, DateRange.cs, Entity.cs, AggregateRoot.cs, Result.cs, Error.cs

Application/
  Expenses/
    RecordExpense/   RecordExpenseCommand.cs, RecordExpenseHandler.cs, RecordExpenseValidator.cs
    GetExpenses/     GetExpensesQuery.cs, GetExpensesHandler.cs
  Abstractions/      ICommand.cs, ICommandHandler.cs, IQuery.cs, IQueryHandler.cs, IUnitOfWork.cs
  Behaviors/         ValidationDecorator.cs, LoggingDecorator.cs

Api/
  Endpoints/Expenses/ RecordExpenseEndpoint.cs, GetExpensesEndpoint.cs
  Infrastructure/     IEndpoint.cs, ResultExtensions.cs (Result -> IResult/ProblemDetails)
```

## Naming
- Commands: imperative verb + noun – `RecordExpenseCommand`, `CompleteStageCommand`.
- Queries: `Get…Query` – `GetBudgetOverviewQuery`.
- Handlers: `<Name>Handler`; validators: `<Name>Validator`.
- Contracts: `<Name>Request` / `<Name>Response`.
- Errors: static classes per aggregate with codes `Aggregate.Reason`, e.g. `ExpenseErrors.NotFound` → `"Expense.NotFound"`.
- Domain events: past tense – `StageCompleted`.
- Strongly-typed IDs: `readonly record struct ExpenseId(Guid Value)` with `New()` using `Guid.CreateVersion7()`.
- Private fields (including `static readonly`): `_camelCase`. Constants: `PascalCase`. Interfaces: `I` prefix. Enforced by `.editorconfig`.

## Domain
- No public setters; collections exposed as `IReadOnlyList<T>`.
- Construction through static factory methods returning `Result<T>` when creation can fail by business rules.
- Guard clauses for programmer errors (null/empty) throw; business rule violations return `Result.Failure`.

## API
- Minimal API endpoints implement `IEndpoint` and are discovered at startup; grouped with `MapGroup("/api/projects/{projectId}/expenses")`.
- Routes: plural nouns, kebab-case, nested only one level under the aggregate root.
- Every endpoint declares `.WithName()`, `.Produces…()` and tags for OpenAPI.
- Errors always as ProblemDetails with `type`, `title`, `status`, `detail`, and `errors` for validation.
- Pagination: `?page=1&pageSize=20`, response `{ items, page, pageSize, totalCount }`.

## Persistence
- One `IEntityTypeConfiguration<T>` per entity in `Infrastructure/Persistence/Configurations`.
- Table names plural; schema `bm`.
- `decimal(18,2)` for money; `rowversion` concurrency token on aggregate roots.
- Migrations named in PascalCase describing the change: `AddExpenseDocuments`.

## Testing
- xUnit v3 on Microsoft.Testing.Platform (`xunit.v3.mtp-v2`); test projects are executables (`OutputType=Exe`).
- A test project is created together with its first test (zero tests fail the run on Microsoft.Testing.Platform).
- Test names: `Method_Scenario_ExpectedResult` – `Complete_WhenStageNotStarted_ReturnsFailure` (CA1707 is disabled under `tests/` for this reason).
- Arrange / Act / Assert separated by blank lines; one behaviour per test.
- Shouldly for assertions, NSubstitute for test doubles, test data builders for aggregates (`ProjectBuilder`).
- Domain: pure unit tests, no mocks. Application: mock ports. Api: integration tests with `WebApplicationFactory` against a dedicated test database (`BuildManager_Tests`), reset between tests.
- Architecture tests guard the dependency rules from ARCHITECTURE.md on project references and IL; a new production project must be added to `DependencyRules.cs`.

## Git
- Trunk-based: short-lived branches `feature/…`, `fix/…`, `docs/…`, merged to `main` by PR. `main` is protected: PR required, CI must pass, no force pushes.
- Conventional Commits: `feat(expenses): record expense with documents`.
- CI (GitHub Actions, `.github/workflows/ci.yml`) on every push to `main` and every PR: restore, format check (`dotnet format --verify-no-changes`), build in Release, test. Run the same commands locally before pushing (see README).
