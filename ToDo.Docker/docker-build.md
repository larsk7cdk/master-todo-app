# Docker Build Instructions

## Available Compose Files

| File | Purpose | Ports |
|------|---------|-------|
| `docker-compose.yml` | Main ToDo stack (SQL Server + Agent Factory API) | 1433, 5000 |
| `docker-sonarqube-compose.yml` | SonarQube code analysis + PostgreSQL | 9000 |

---

## Quick Start — ToDo Stack

```powershell
cd C:\github\larsk7cdk\AAU\masterthesis\ToDo-development\ToDo.Docker

# Start all services (builds images if needed)
docker compose -f docker-compose.yml up -d --build

# Stop all services
docker compose -f docker-compose.yml down

# Stop and remove volumes (full reset — DELETES ALL DATA)
docker compose -f docker-compose.yml down -v
```

Access points:
- **Agent Factory API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **SQL Server**: `localhost,1433` (user: `sa`, password: `P@ssword2026`)

See [docker-compose-README.md](docker-compose-README.md) for detailed documentation.

---

## Quick Start — SonarQube

```powershell
cd C:\github\larsk7cdk\AAU\masterthesis\ToDo-development\ToDo.Docker

# Start SonarQube
docker compose -f docker-sonarqube-compose.yml up -d

# Stop SonarQube
docker compose -f docker-sonarqube-compose.yml down

# Stop and remove volumes (full reset — DELETES ALL DATA)
docker compose -f docker-sonarqube-compose.yml down -v
```

Access points:
- **SonarQube UI**: http://localhost:9000
- **Default credentials**: `admin` / `admin` (you will be prompted to change on first login)

> **Linux note**: SonarQube requires `vm.max_map_count >= 524288`.
> Run: `sudo sysctl -w vm.max_map_count=524288`

---

## Manual Docker Build Instructions

### Important: Build Context

All Docker images **must be built from the repository root** (`ToDo-development/`), not from individual project directories.

### SQL Server

SQL Server uses the official Microsoft image — no build required. Run the container directly:

```powershell
# Create a volume for persistent data
docker volume create sqlserver-data

# Run SQL Server container
docker run -d `
  --name ToDo-sqlserver `
  -e "ACCEPT_EULA=Y" `
  -e "MSSQL_SA_PASSWORD=P@ssword2026" `
  -e "MSSQL_PID=Developer" `
  -p 1433:1433 `
  -v sqlserver-data:/var/opt/mssql `
  mcr.microsoft.com/mssql/server:2022-latest

# Check logs
docker logs ToDo-sqlserver
```

**Connection string:**
```
Server=localhost,1433;Database=ToDo-db;User Id=sa;Password=P@ssword2026;TrustServerCertificate=True;
```

> **Note**: Ensure port 1433 is not already in use on your host machine.

### Agent Factory API

```powershell
# Navigate to repository root
cd C:\github\larsk7cdk\AAU\masterthesis\ToDo-development

# Build the image
docker build -f ToDo.Factory/src/ToDo.API/Dockerfile -t ToDo-factory:latest .

# Run the container
docker run -d -p 5000:5000 --name agentfactory ToDo-factory:latest
```

---

## Troubleshooting

### "No ports exposed in this image"

You are likely running the build from the wrong directory.
**Solution**: Always build from the repository root (`ToDo-development/`).

### Build Context Issue

The Dockerfile copies files using paths like:
```dockerfile
COPY ["ToDo.Factory/src/ToDo.API/...", "..."]
```
These paths only exist when building from the repository root.

### Verify Exposed Ports

```powershell
docker inspect ToDo-factory:latest | Select-String -Pattern "ExposedPorts"
```

Expected output:
```json
"ExposedPorts": {
    "5000/tcp": {}
}
```

### Check Port Conflicts

```powershell
netstat -ano | findstr "1433 5000 9000"
```

### SQL Server Takes Too Long to Start

SQL Server can take 30–60 seconds to initialise. The Agent Factory API will wait for the health check to pass before starting. Check status with:

```powershell
docker compose -f docker-compose.yml ps
```