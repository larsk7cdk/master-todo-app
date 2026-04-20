# Docker Compose Setup

This directory contains Docker Compose configuration to run the ToDo stack locally.

## Compose Files

| File | Purpose |
|------|---------|
| `docker-compose.yml` | Main ToDo stack (SQL Server + Agent Factory API) |
| `docker-sonarqube-compose.yml` | SonarQube code analysis + PostgreSQL |

See [docker-build.md](docker-build.md) for manual build instructions.

---

## Project Name

The Docker Compose project is named **`ToDo`** (configured in the `name:` property). This means:
- Containers will be named: `ToDo-<service>` (e.g., `ToDo-sqlserver`)
- Images will be named: `ToDo-<service>` (e.g., `ToDo-factory`)
- Network will be named: `ToDo_ToDo-network`

You can override this with the `-p` flag:
```powershell
docker compose -f docker-compose.yml -p myproject up -d
```

---

## Services

- **SQL Server** — Microsoft SQL Server 2022 Developer Edition (port 1433)
- **Agent Factory API** — ASP.NET Core backend API (port 5000)

## Prerequisites

- Docker Desktop installed and running
- At least 4 GB of RAM available for Docker
- Ports 1433 and 5000 available on your host machine

---

## Quick Start

### Start All Services

```powershell
# Navigate to the Docker directory
cd C:\github\larsk7cdk\AAU\masterthesis\ToDo-development\ToDo.Docker

# Start all services (builds images if needed)
docker compose -f docker-compose.yml up -d --build

# View logs for all services
docker compose -f docker-compose.yml logs -f

# View logs for a specific service
docker compose -f docker-compose.yml logs -f agentfactory
```

### Stop All Services

```powershell
# Stop all services (keeps data volumes)
docker compose -f docker-compose.yml down

# Stop and remove everything including volumes (DELETES ALL DATA)
docker compose -f docker-compose.yml down -v
```

### Rebuild Services

```powershell
# Rebuild all images
docker compose -f docker-compose.yml build

# Rebuild a specific service
docker compose -f docker-compose.yml build agentfactory

# Rebuild and restart
docker compose -f docker-compose.yml up -d --build
```

---

## Access Points

| Service | URL / Address |
|---------|--------------|
| Agent Factory API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |
| SQL Server | `localhost,1433` |

## SQL Server Connection

| Field | Value |
|-------|-------|
| Server | `localhost,1433` |
| User | `sa` |
| Password | `P@ssword2026` |
| Database | `ToDo-db` |

> **Security note**: The default password is for development only. Change it before deploying to production.

---

## Troubleshooting

### Services Won't Start

1. Confirm Docker Desktop is running.
2. Check for port conflicts:
   ```powershell
   netstat -ano | findstr "1433 5000"
   ```
3. Check container logs:
   ```powershell
   docker compose -f docker-compose.yml logs
   ```

### SQL Server Connection Issues

SQL Server can take 30–60 seconds to fully initialise. The API will wait for the health check to pass before starting.

Check health status:
```powershell
docker compose -f docker-compose.yml ps
```

Verify SQL Server directly:
```powershell
docker exec ToDo-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "P@ssword2026" -C -Q "SELECT 1"
```

### API Service Not Starting

- The API depends on SQL Server being healthy — wait for SQL Server to pass its health check.
- Check if database migrations are needed.
- Verify the connection string in the API logs.

---

## Development Workflow

### Updating Backend Code

```powershell
# Rebuild and restart only the factory service
docker compose -f docker-compose.yml up -d --build agentfactory
```

### Database Migrations

```powershell
# Connect to the factory container and run migrations
docker exec -it ToDo-factory dotnet ef database update
```

---

## Clean Start

To start completely fresh:

```powershell
# Stop everything and remove volumes
docker compose -f docker-compose.yml down -v

# Remove all built images
docker compose -f docker-compose.yml down --rmi all

# Start fresh
docker compose -f docker-compose.yml up -d --build
```

---

## Environment Variables

You can override default settings by creating a `.env` file in this directory:

```env
# SQL Server
MSSQL_SA_PASSWORD=P@ssword2026
SQL_PORT=1433

# API
FACTORY_PORT=5000
```

---

## Network Architecture

All services run in the `ToDo-network` bridge network:
- Services communicate using their service names (e.g., `sqlserver`, `agentfactory`)
- SQL Server is reachable from the API via `Server=sqlserver;...` in the connection string

---

## Production Considerations

This setup is for **development only**. For production:

1. Change all default passwords
2. Use secrets management (Docker secrets, Azure Key Vault, etc.)
3. Enable HTTPS/TLS
4. Configure proper CORS policies
5. Use a production-grade SQL Server license (not Developer Edition)
6. Implement proper logging and monitoring
7. Set up container orchestration (Kubernetes, Azure Container Apps, etc.)
8. Configure resource limits and health checks
9. Use multi-stage builds with security scanning
10. Implement proper backup strategies for data volumes