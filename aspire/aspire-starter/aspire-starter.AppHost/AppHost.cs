var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");


var apiService = builder.AddProject<Projects.aspire_starter_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.aspire_starter_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
