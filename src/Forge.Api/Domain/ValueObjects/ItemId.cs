using Forge.Api.Domain.Abstractions;

namespace Forge.Api.Domain.ValueObjects;

public readonly record struct ItemId(Guid Value) : IStronglyTypedId<ItemId>
{
    public static ItemId Create() => new(Guid.CreateVersion7());
    public static ItemId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
