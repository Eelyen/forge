namespace Forge.Api.Domain.ValueObjects;

public readonly record struct PlanId(Guid Value)
{
    public static PlanId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
