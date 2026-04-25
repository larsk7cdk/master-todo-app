using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using ToDo.Persistence.DatabaseContext;

namespace ToDo.IntegrationsTest.Shared;

public class DatabaseFixture
{
    private readonly MsSqlContainer _sqlContainer = PodmanDockerHostPatcher.EnsurePatched()
        ? new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithName("sqlserver-test-" + Guid.NewGuid())
            .Build()
        : throw new InvalidOperationException("Docker host patch failed.");

    public string ConnectionString => _sqlContainer.GetConnectionString();


    public async ValueTask StartAsync()
    {
        await _sqlContainer.StartAsync();
        await ApplyMigrationsAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
    }

    private async Task ApplyMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<AppDatabaseContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        await using var db = new AppDatabaseContext(options);
        await db.Database.MigrateAsync();
        await db.Database.EnsureCreatedAsync();
    }
}