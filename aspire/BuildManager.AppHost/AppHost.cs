var builder = DistributedApplication.CreateBuilder(args);

// Local SQL Server instance; the value comes from AppHost user secrets (ConnectionStrings:buildmanager).
var database = builder.AddConnectionString("buildmanager");

builder.AddProject<Projects.BuildManager_Api>("api")
    .WithReference(database)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.BuildManager_Web>("web")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
