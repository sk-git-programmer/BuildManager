# ADR 0002: Clean Architecture with separate Api and Web

- **Status:** Accepted
- **Date:** 2026-11-17

## Context

BuildManager has non-trivial business rules (budgets, stages, payments, loan tranches) that should be testable
without a database or HTTP. The UI is expected to evolve independently of the backend, and the project is also a
portfolio piece meant to show deliberate layering and testability.

## Decision

- Split the solution into `Domain`, `Application`, `Infrastructure`, `Contracts`, `Api`, `Web` and `Web.Client`.
- Dependencies point inwards: `Domain` has no dependencies, `Application` depends only on `Domain`,
  `Infrastructure` implements ports defined in `Application`.
- `Api` is the composition root and the only backend contract. `Web` / `Web.Client` depend only on `Contracts`
  and talk to the API over HTTP.
- The dependency rules are documented in ARCHITECTURE.md and enforced by architecture tests (ADR 0007).

## Consequences

- Business rules are tested as plain unit tests; infrastructure can be replaced behind ports.
- The UI can be rewritten or complemented (e.g. a mobile client) without touching the backend.
- More projects and explicit mapping between Contracts, commands/queries and domain types.
- Cross-cutting setup lives in the composition root (`Api`), not spread across layers.
