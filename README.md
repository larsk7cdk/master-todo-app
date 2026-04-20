# ToDo

A master thesis project implementing an AI-assisted factory API built with .NET 10 and .NET Aspire. The solution follows Clean Architecture principles with a shared kernel pattern, enabling modular and testable microservice-style development.

## Solution Structure

```
ToDo/
├── ToDo.Hosting/
│   └── ToDo.AppHost/             # .NET Aspire orchestration host
├── ToDo.Factory/
│   ├── src/
│   │   ├── ToDo.API/         # ASP.NET Core Web API
│   │   ├── ToDo.Application/ # Application layer (use cases, handlers)
│   │   ├── ToDo.Domain/      # Domain models
│   │   ├── ToDo.Infrastructure/  # External service integrations
│   │   └── ToDo.Persistence/ # EF Core, migrations, repositories
│   └── tests/
│       └── ToDo.IntegrationsTest/ # Integration tests (xUnit + Testcontainers)
└── ToDo.Shared/
    └── src/
        ├── ToDo.ServiceDefaults/     # Aspire service defaults (telemetry, health checks)
        ├── ToDo.Shared/              # Shared utilities
        ├── ToDo.Shared.API/          # Shared API infrastructure (base controller, middleware)
        ├── ToDo.Shared.Application/  # Shared application abstractions (IRequestHandler, ICrudRepository)
        ├── ToDo.Shared.Domain/       # Shared domain base types (BaseModel)
        ├── ToDo.Shared.Infrastructure/ # Shared infrastructure helpers
        └── ToDo.Shared.Persistence/  # Shared EF Core base context and repositories
```

## Architecture

The solution uses **Clean Architecture** layered as follows:

- **API** — HTTP controllers, DTOs, request validation via FluentValidation, Swagger/OpenAPI
- **Application** — `IRequestHandler<TIn, TOut>` use cases, auto-discovered and registered from assemblies
- **Domain** — Pure domain models with no external dependencies
- **Infrastructure** — External integrations (AI services, third-party APIs)
- **Persistence** — Entity Framework Core with SQL Server, generic `CrudRepository<T>`, audit timestamps via `BaseAppDatabaseContext`

Shared cross-cutting concerns (exception handling, base controller, service discovery, OpenTelemetry) live in the `ToDo.Shared.*` projects and are consumed by each service.

## Technology Stack

| Concern | Technology |
|---|---|
| Framework | .NET 10 / ASP.NET Core |
| Orchestration | .NET Aspire 13 |
| Database | SQL Server (via Docker) |
| ORM | Entity Framework Core |
| Validation | FluentValidation |
| Observability | OpenTelemetry (traces, metrics, logs) |
| API Documentation | Swagger / OpenAPI |
| Testing | xUnit v3, Testcontainers (MsSql), FluentAssertions |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (required for SQL Server container and integration tests)
- .NET Aspire workload: `dotnet workload install aspire`

### Run the Application

Start the Aspire AppHost to launch all services and the SQL Server container:

```bash
dotnet run --project ToDo.Hosting/ToDo.AppHost
```

The Aspire dashboard will be available at `http://localhost:15888` (or as printed in the console). The Factory API Swagger UI is accessible at the `/swagger` endpoint of the service.

### Configuration

The AppHost reads SQL Server parameters from `ToDo.Hosting/ToDo.AppHost/appsettings.json`:

```json
{
  "Parameters": {
    "sql-container-name": "ToDo-sqlserver",
    "sql-db-name": "ToDo-db",
    "sql-data-volume": "ToDo-sqlserver-data",
    "sql-password": "<your-password>"
  }
}
```

> **Note:** Do not commit real passwords. Use user secrets in development:
> ```bash
> dotnet user-secrets set "Parameters:sql-password" "<your-password>" --project ToDo.Hosting/ToDo.AppHost
> ```

### Run Tests

Integration tests spin up a real SQL Server instance via Testcontainers. Docker must be running.

```bash
dotnet test ToDo.Factory/tests/ToDo.IntegrationsTest
```

## Key Design Patterns

- **IRequestHandler** — Lightweight command/query pattern; handlers are auto-registered from assemblies, avoiding a full MediatR dependency.
- **ICrudRepository\<TModel\>** — Generic repository interface for standard CRUD operations.
- **BaseAppDatabaseContext** — EF Core base context that automatically sets `DateCreated` and `DateModified` audit timestamps.
- **Shared Middleware** — `ExceptionMiddleware`, `ValidationExceptionHandler`, and `GlobalExceptionHandler` provide consistent ProblemDetails error responses.
- **Service Defaults** — A shared Aspire project that wires up OpenTelemetry, health checks, service discovery, and HTTP resilience for all services.