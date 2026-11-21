# ADR 0005: Result pattern for business errors

- **Status:** Accepted
- **Date:** 2026-11-17

## Context

Many operations can fail for expected business reasons: completing a stage that has not started, recording a
payment date in the future, an invalid NIP. Using exceptions for these makes failure paths invisible in method
signatures, costs performance and mixes expected outcomes with real faults.

## Decision

- Expected business failures are returned as `Result` / `Result<T>` carrying an
  `Error(Code, Description, ErrorType)`.
- Error codes follow `Aggregate.Reason` (e.g. `Expense.NotFound`) and are defined in static `<Aggregate>Errors` classes.
- Exceptions are reserved for programmer errors (guard clauses for null/empty arguments) and infrastructure failures.
- The API maps `ErrorType` to RFC 9457 ProblemDetails: Validation → 400, NotFound → 404, Conflict → 409,
  Forbidden → 403.

## Consequences

- Failure paths are explicit in signatures and must be handled by callers.
- Business errors have stable codes that the UI and tests can rely on.
- Slightly more verbose code (checking results instead of letting exceptions propagate).
- A single place in the API translates errors to HTTP responses consistently.
