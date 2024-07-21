var builder = DistributedApplication.CreateBuilder(args);



//var cosmos = builder.AddAzureCosmosDB("Cosmos");
//var cosmosdb = cosmos.AddDatabase("shortestRouteDb");

var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");  

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
