using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Application.Interfaces;
using ToDo.Application.Models;
using ToDo.Persistence.DatabaseContext;
using ToDo.Persistence.Repositories;


namespace ToDo.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDatabaseContext>(options =>
        {
            // Aspire injects connection strings with the resource name from AppHost
            // In AppHost.cs, you have: .AddDatabase(sqlAppDbName)
            // Use the database name from configuration or use the resource name directly
            var connectionString =
                configuration.GetConnectionString(configuration["Parameters:sql-db-name"] ?? "appdb");
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<ICrudRepository<ToDoModel>, ToDoRepository>();

        return services;
    }
}