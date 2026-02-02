namespace Forge.Api.Domain.Abstractions;

public abstract class Entity<TId> : IAuditable
{
    public TId Id { get; protected set; } = default!;

    public DateTimeOffset CreatedUtc { get; private set; }
    public DateTimeOffset UpdatedUtc { get; private set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "Intential design for DbContext auditing")]
    void IAuditable.SetCreated(DateTimeOffset nowUtc)
    {
        CreatedUtc = nowUtc;
        UpdatedUtc = nowUtc;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "Intential design for DbContext auditing")]
    void IAuditable.Touch(DateTimeOffset nowUtc)
    {
        if (CreatedUtc == default)
            CreatedUtc = nowUtc;

        UpdatedUtc = nowUtc;
    }
}
