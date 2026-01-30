namespace Forge.Api.Features.System;

public static class Ping
{
    public sealed record Query(string? Name);
    public sealed record Result(string Message);

    public sealed class Handler
    {
        public static Result Handle(Query query)
            => new($"pong{(string.IsNullOrWhiteSpace(query.Name) ? string.Empty : $", {query.Name}")}!");
    }
}
