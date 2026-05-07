using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using ToDo.Persistence.DatabaseContext;

namespace ToDo.E2ETest.Shared;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = PodmanDockerHostPatcher.EnsurePatched()
        ? new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithName("sqlserver-test-" + Guid.NewGuid())
            .WithPassword("P@ssword2026")
            .Build()
        : throw new InvalidOperationException("Docker host patch failed.");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AppDatabaseContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDatabaseContext>(options => { options.UseSqlServer(_sqlContainer.GetConnectionString()); });
        });
    }

    public async ValueTask InitializeAsync()
    {
        await _sqlContainer.StartAsync();
       await ApplyMigrationsAsync<AppDatabaseContext>();
    }

    public new async ValueTask DisposeAsync()
    {
        await _sqlContainer.StopAsync();
        GC.SuppressFinalize(this);
    }

    private async Task ApplyMigrationsAsync<TDbContext>() where TDbContext : DbContext
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await db.Database.MigrateAsync();
    }
}
