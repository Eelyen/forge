using Forge.Api.Domain.Abstractions;

namespace Forge.Api.Domain.ValueObjects;

public readonly record struct BuildingId(Guid Value) : IStronglyTypedId<BuildingId>
{
    public static BuildingId New() => new(Guid.CreateVersion7());
    public static BuildingId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}