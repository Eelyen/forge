using Forge.Api.Features.System;
using Forge.Api.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Infrastructure
// --------------------

builder.Services.AddDbContext<ForgeDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("ForgeDb") ?? "Data Source=forge.db");
});

// --------------------
// Wolverine
// --------------------

builder.Host.UseWolverine();

// --------------------
// App
// --------------------

var app = builder.Build();

// Early database creation for dev purposes
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ForgeDbContext>();
    db.Database.EnsureCreated();
}


app.MapGet("/ping", (string? name, IMessageBus bus) => bus.InvokeAsync<Ping.Result>(new Ping.Query(name)));

app.MapGet("/", () => "Forge API is running.");

app.Run();
