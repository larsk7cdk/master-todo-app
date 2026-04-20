# AI Assist Factory persistence

## Start a SQL Server in docker

### Create a volume for persistent data if it doesn't exist

podman volume create sqlserver-data

### Run SQL Server container

podman run -d `
  --name ToDo-sqlserver `
-e "ACCEPT_EULA=Y" `
  -e "MSSQL_SA_PASSWORD=P@ssword2026" `
-e "MSSQL_PID=Developer" `
  -p 1433:1433 `
-v sqlserver-data:/var/opt/mssql `
mcr.microsoft.com/mssql/server:2022-latest

---

## Database Migrations

```bash
cd ToDo.Factory/src/ToDo.Persistence

# Create new migration
dotnet ef migrations add MigrationName --startup-project ../ToDo.API/ToDo.API.csproj

# Update the database
dotnet ef database update --startup-project ../ToDo.API/ToDo.API.csproj
```


