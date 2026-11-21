# ADR 0006: Blazor Web App with interactive WebAssembly and MudBlazor

- **Status:** Accepted
- **Date:** 2026-11-17

## Context

The UI should be written in C# (one language across the stack), should treat the API as its only backend
contract, and should later support a BFF so that no authentication tokens live in the browser (Phase 6).

## Decision

- Use a Blazor Web App with the **interactive WebAssembly** render mode applied globally, split into two projects:
  - `BuildManager.Web.Client` runs in the browser: components, pages and typed API clients. It must never contain
    secrets or server-side logic; the separate project enforces this at compile time.
  - `BuildManager.Web` is the ASP.NET Core host: serves static assets and the WebAssembly bundle, prerenders the first
    page, uses ServiceDefaults for telemetry and health checks, and becomes the BFF (YARP proxy + cookie auth) in Phase 6.
- Use MudBlazor as the component library (added in Phase 5).
- The client talks to the API over HTTP using `Contracts` only.

## Consequences

- One C# stack; DTOs in `Contracts` are shared between the API and the browser.
- The API remains the single backend contract; the UI does not access the database or Application layer.
- Prerendering gives a fast first paint but runs client components on the server first: typed API clients must also
  be registered in the Web host, or prerendering must be disabled for data-loading pages (to decide in Phase 5).
- Larger initial download than server-side Blazor; mitigated by prerendering.
- Rejected alternative: Blazor WebAssembly Standalone (one project, static files only) – simpler, but no prerendering
  and no natural place for the BFF.
