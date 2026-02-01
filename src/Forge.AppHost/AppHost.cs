var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Forge_Api>("forge-api");
var web = builder.AddProject<Projects.Forge_Web>("forge-web")
    .WithReference(api);

builder.Build().Run();
