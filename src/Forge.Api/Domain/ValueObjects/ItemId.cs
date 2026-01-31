namespace Forge.Api.Domain.ValueObjects;

public readonly record struct ItemId(Guid Value)
{
    public static ItemId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
