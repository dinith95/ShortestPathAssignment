var builder = DistributedApplication.CreateBuilder(args);

//var redis = builder.AddRedis("redis");
//redis.WithEndpoint("localhost", e => e.TargetPort = 6369);

var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");
                                               // .WithReference(redis);

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
