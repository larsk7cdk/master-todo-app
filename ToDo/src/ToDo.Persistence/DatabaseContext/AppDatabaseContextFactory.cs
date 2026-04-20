using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ToDo.Persistence.DatabaseContext;

public class AppDatabaseContextFactory : IDesignTimeDbContextFactory<AppDatabaseContext>
{
    public AppDatabaseContext CreateDbContext(string[] args)
    {
        // Find roden til projektet, typisk hvor appsettings.json ligger
        var basePath = Directory.GetCurrentDirectory();

        // Byg konfigurationen
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Læs connection string
        var connectionString = configuration.GetConnectionString("appdb");

        var optionsBuilder = new DbContextOptionsBuilder<AppDatabaseContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDatabaseContext(optionsBuilder.Options);
    }
}