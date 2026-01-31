namespace Forge.Api.Domain.Abstractions;

public abstract class Entity<TId> : IAuditable
{
    public TId Id { get; protected set; } = default!;

    public DateTimeOffset CreatedUtc { get; private set; }
    public DateTimeOffset UpdatedUtc { get; private set; }

    void IAuditable.SetCreated(DateTimeOffset nowUtc)
    {
        CreatedUtc = nowUtc;
        UpdatedUtc = nowUtc;
    }

    void IAuditable.Touch(DateTimeOffset nowUtc)
    {
        if (CreatedUtc == default)
            CreatedUtc = nowUtc;

        UpdatedUtc = nowUtc;
    }
}
