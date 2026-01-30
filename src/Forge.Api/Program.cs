using Forge.Api.Features.System;

using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWolverine();

var app = builder.Build();

app.MapGet("/ping", (string? name, IMessageBus bus) => bus.InvokeAsync<Ping.Result>(new Ping.Query(name)));

app.MapGet("/", () => "Forge API is running.");

app.Run();
