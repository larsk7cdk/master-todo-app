# ToDo

A ToDo blueprint project built with .NET 10 and .NET Aspire. The solution follows Clean Architecture principles with a shared kernel pattern,
providing a foundation for modular and testable microservice-style development.

## Solution Structure

```
ToDoApp/
├── ToDo.Aspire/
│   ├── ToDo.AppHost/             # .NET Aspire orchestration host
│   └── ToDo.ServiceDefaults/     # Aspire service defaults (telemetry, health checks, resilience)
├── ToDo/
│   ├── src/
│   │   ├── ToDo.API/             # ASP.NET Core Web API
│   │   ├── ToDo.Application/     # Application layer (exceptions, validation)
│   │   ├── ToDo.Domain/          # Domain models (empty — ready for entities)
│   │   ├── ToDo.Infrastructure/  # External service integrations (empty — ready for implementations)
│   │   └── ToDo.Persistence/     # EF Core, migrations, database context
│   └── tests/
│       ├── ToDo.E2ETest/         # End-to-end tests
│       ├── ToDo.IntegrationsTest/ # Integration tests (xUnit + Testcontainers)
│       └── ToDo.UnitTest/        # Unit tests
├── ToDo.Shared/                  # Placeholder for future shared libraries
└── ToDo.Docker/                  # Docker Compose setup
```

## Architecture

The solution uses **Clean Architecture** layered as follows:

- **API** — HTTP controllers, DTOs, request validation via FluentValidation, Swagger/OpenAPI, lowercase route conventions
- **Application** — Shared exceptions (`BadRequestException`, `NotFoundException`) and FluentValidation registration
- **Domain** — Pure domain models with no external dependencies (empty blueprint layer)
- **Infrastructure** — External integrations (empty blueprint layer, ready for AI services, third-party APIs, etc.)
- **Persistence** — Entity Framework Core with SQL Server, `BaseAppDatabaseContext<TContext>` with assembly-based configuration scanning, audit
  timestamp support

Cross-cutting concerns (exception handling, base controller, service discovery, OpenTelemetry) live in `ToDo.ServiceDefaults` and are consumed by each
service.

## Technology Stack

| Concern           | Technology                                         |
|-------------------|----------------------------------------------------|
| Framework         | .NET 10 / ASP.NET Core                             |
| Orchestration     | .NET Aspire 13.1.2                                 |
| Database          | SQL Server (via Docker)                            |
| ORM               | Entity Framework Core 10                           |
| Validation        | FluentValidation 12                                |
| Observability     | OpenTelemetry (traces, metrics, logs)              |
| API Documentation | Swagger / OpenAPI (Swashbuckle 10)                 |
| Testing           | xUnit v3, Testcontainers (MsSql), FluentAssertions |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (required for SQL Server container and integration tests)
- .NET Aspire workload: `dotnet workload install aspire`

### Run the Application

Start the Aspire AppHost to launch all services and the SQL Server container:

```bash
dotnet run --project ToDo.Aspire/ToDo.AppHost
```

The Aspire dashboard will be available at `http://localhost:15888` (or as printed in the console). The API Swagger UI is accessible at the `/swagger`
endpoint of the service.

### Configuration

The AppHost reads SQL Server parameters from `ToDo.Aspire/ToDo.AppHost/appsettings.json`:

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
> dotnet user-secrets set "Parameters:sql-password" "<your-password>" --project ToDo.Aspire/ToDo.AppHost
> ```

### Run Tests

```bash
# Unit tests
dotnet test ToDo/tests/ToDo.UnitTest

# Integration tests (requires Docker)
dotnet test ToDo/tests/ToDo.IntegrationsTest

# End-to-end tests (requires Docker)
dotnet test ToDo/tests/ToDo.E2ETest
```

Integration and E2E tests spin up a real SQL Server instance via Testcontainers. Docker must be running.

## Key Design Patterns

- **BaseAppDatabaseContext\<TContext\>** — EF Core base context with generic type parameter and `ApplyConfigurationsFromAssembly` for convention-based
  entity configuration. Wired for `DateCreated`/`DateModified` audit timestamps.
- **Shared Middleware** — `ExceptionMiddleware`, `ValidationExceptionHandler`, and `GlobalExceptionHandler` provide consistent `ProblemDetails` error
  responses.
- **Service Defaults** — A shared Aspire project that wires up OpenTelemetry, health checks, service discovery, and HTTP resilience for all services.
- **Lowercase Routes** — `LowerCaseParameterTransformer` applied globally for consistent API URL casing.
