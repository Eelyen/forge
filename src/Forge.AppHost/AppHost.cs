var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlite("forgeDb", null, "forge.db")
                .WithSqliteWeb();

var api = builder.AddProject<Projects.Forge_Api>("forge-api")
                 .WithReference(db);

var web = builder.AddProject<Projects.Forge_Web>("forge-web")
                 .WithReference(api);

builder.Build().Run();
