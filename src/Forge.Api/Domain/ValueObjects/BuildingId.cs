namespace Forge.Api.Domain.ValueObjects;

public readonly record struct BuildingId(Guid Value)
{
    public static BuildingId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}