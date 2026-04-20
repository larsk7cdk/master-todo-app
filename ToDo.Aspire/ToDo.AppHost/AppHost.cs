using System;
using Aspire.Hosting;
using Projects;
using static System.Console;

var builder = DistributedApplication.CreateBuilder(args);

// Read from SQL parameters configuration
var sqlContainerName = builder.Configuration["Parameters:sql-container-name"]!;
var sqlDbName = builder.Configuration["Parameters:sql-db-name"]!;
var sqlDataVolume = builder.Configuration["Parameters:sql-data-volume"]!;
var sqlPassword = builder.AddParameter("sql-password", secret: true);

// Add SQL Server container with persistent volume and fixed port
var sqlServer = builder.AddSqlServer("sqlservice", password: sqlPassword)
    .WithContainerName(sqlContainerName)
    .WithDataVolume(sqlDataVolume) // Persist data between restarts
    .WithHostPort(1433) // Fix to host port 1433
    .WithEndpointProxySupport(false)
    .AddDatabase(sqlDbName);

// Add API
builder.AddProject<ToDo_API>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);


// Run the application
try
{
    var app = builder.Build();
    await app.RunAsync();
}
catch (Exception e)
{
    WriteLine("Application error: {0}", e.Message);
}