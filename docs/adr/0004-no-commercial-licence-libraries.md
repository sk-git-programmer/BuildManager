# ADR 0004: No MediatR, AutoMapper or FluentAssertions

- **Status:** Accepted
- **Date:** 2026-11-17

## Context

MediatR, AutoMapper and FluentAssertions moved to commercial licences in 2025. The project should be free to use,
fork and run by anyone, and should not depend on licence terms that may change again. The value these libraries
add here is small compared to the cost of the dependency.

## Decision

- **CQRS without a mediator:** own `ICommand` / `IQuery` and `ICommandHandler` / `IQueryHandler` interfaces.
  Handlers are injected directly into endpoints. Validation and logging are decorators registered with Scrutor.
- **Mapping:** written by hand (extension methods or static mapping methods next to the types they map).
- **Assertions:** Shouldly.

## Consequences

- No licence risk; dependencies stay small and well understood.
- Explicit mapping is more code, but it is visible, debuggable and checked by the compiler.
- Without a mediator there is no generic pipeline; every cross-cutting concern is an explicit decorator, which
  keeps the request flow easy to follow.
- Contributors used to MediatR need to learn the (small) local abstractions in `Application/Abstractions`.
