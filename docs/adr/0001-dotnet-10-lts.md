# ADR 0001: .NET 10 LTS

- **Status:** Accepted
- **Date:** 2026-11-17

## Context

The project starts in November 2026. .NET 8 (LTS) and .NET 9 (STS) reached end of support on 10 November 2026.
.NET 10 is the current Long Term Support release (supported until November 2028). .NET 11 has been released as a
Standard Term Support version, and the development machine also has the .NET 11 SDK installed.

The project should run on a supported runtime for its whole planned life without forced upgrades in the middle of
the roadmap, and every machine (developer and CI) must build with the same SDK line.

## Decision

- Target `net10.0` in all projects (set once in `Directory.Build.props`).
- Pin the SDK in `global.json` to `10.0.404` with `rollForward: latestFeature` and `allowPrerelease: false`:
  newer 10.0.x feature bands and patches are used automatically, the .NET 11 SDK never is.
- Keep Microsoft.Extensions.* and ASP.NET Core packages on the 10.x line, even when 11.x versions are available.
- CI installs the SDK from `global.json` (`actions/setup-dotnet` with `global-json-file`).

## Consequences

- Supported runtime until November 2028, which covers the roadmap.
- C# 14 is available without setting `LangVersion`.
- Some 11.x package versions on NuGet are not usable (framework packages target `net11.0`); versions must be
  chosen deliberately in `Directory.Packages.props`.
- Moving to the next LTS (.NET 12) is a separate, deliberate decision with its own ADR.
