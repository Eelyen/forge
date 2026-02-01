using Forge.Api.Features.System;
using Forge.Api.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Aspire / shared defaults (telemtry, health checks, etc)
builder.AddServiceDefaults();

// OpenAI generation
builder.Services.AddOpenApi();

// Infrastructure
builder.AddSqliteDbContext<ForgeDbContext>("forgeDb");

// Wolverine
builder.Host.UseWolverine();

// App
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    await app.Services.InitializeDatabaseAsync();
}

// Aspire defaults
app.MapDefaultEndpoints();

// App endpoints
app.MapGet("/ping", (string? name, IMessageBus bus) => bus.InvokeAsync<Ping.Result>(new Ping.Query(name)));

app.MapGet("/", () => "Forge API is running.");

app.Run();
