# AGENTS.md

This file provides guidance to AI Agents when working with code in this repository.

## What this project is

THIARA (the binary/csproj name) is a **TARA tool** — Threat Analysis and Risk Assessment — for automotive/embedded security engineering. It implements the ISO 21434 workflow: users define items, identify assets, create damage scenarios rated by CIA impact, link threat scenarios (STRIDE categories), and detail attack paths with feasibility ratings.

## Commands

All commands run from the repo root. The project file lives at `src/thiara.csproj`.

```bash
# Restore dependencies
dotnet restore src/thiara.csproj

# Build
dotnet build src/thiara.csproj

# Run locally (dev mode; auto-applies migrations on startup)
dotnet run --project src/thiara.csproj

# Add an EF Core migration after model changes
dotnet ef migrations add <MigrationName> --project src/thiara.csproj

# Apply migrations manually (also happens automatically on app start)
dotnet ef database update --project src/thiara.csproj
```

There are no tests. CI only builds (`dotnet build --configuration Release`).

**First-run registration**: on a fresh DB with no users, the app prints a one-time registration URL to stdout. Subsequent users are invited via the in-app flow.

## Architecture

### Stack
- **ASP.NET Core 10 / Blazor Server** with Interactive Server render mode
- **Microsoft FluentUI** component library for all UI
- **EF Core + SQLite** (connection string from `appsettings.json`; `DataSource=Data/app.db`)
- Migrations are applied automatically at startup via `db.Database.MigrateAsync()`

### Domain model (hierarchical)
```
Project
  └── ItemDefinition        (the item under analysis, with images)
        └── Asset           (data/function asset, tagged)
              └── DamageScenario   (CIA impact ratings: Safety/Financial/Operational/Privacy)
                    └── ThreatScenario   (STRIDE category, risk value)
                          └── AttackPath   (feasibility factors: elapsed time, expertise, equipment…)
                                └── AttackStep   (ordered list of steps)
```
Tags belong directly to a Project and are associated with Assets.

### Access control
`AccessControl` is a join table between `ApplicationUser` and `Project` with four boolean flags: `ReadAccess`, `WriteAccess`, `Manage`, `Owner`. Every service method calls `AccessControlService` before touching data. `SessionService` resolves the currently authenticated user from `AuthenticationStateProvider`.

### Service layer
Services in `src/Data/Services/` implement `IDataService<T>` (`Save`, `GetItemByIdAsync`, `GetItemsProvider`, `Delete`, `GetItems`). They are registered as **Transient** and receive an `IDbContextFactory<ApplicationDbContext>` — each method creates and disposes its own context. Do not inject `ApplicationDbContext` directly; always use the factory.

`RegisterServices.cs` contains a C# extension method (`AddDatabaseServices()`) that registers all domain services in one call.

### Razor components
Pages live under `src/Components/Pages/<Domain>/`. The pattern is:
- `*Table.razor` — paginated grid using `FluentDataGrid` with a `GridItemsProvider<T>`
- `*Page.razor` — detail/edit view for a single entity
- `src/Components/Dialogs/` — shared modal dialogs (deletion confirmation, image viewer, invite user, etc.)
- `src/Components/GenericAutosave.razor` — wraps forms with debounced auto-save

### Docker
Production deployment mounts `/srv/thiara/data` → `/app/Data` so the SQLite file persists across container restarts. The connection string must point to `/app/Data/app.db`.

### Enums
All rating/category enums (STRIDE, impact values, attack feasibility factors) are in `src/Data/Enums/`. Add new enum values there and create a migration if the values are stored as integers in the DB.
