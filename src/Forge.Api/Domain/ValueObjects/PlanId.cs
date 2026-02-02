using Forge.Api.Domain.Abstractions;

namespace Forge.Api.Domain.ValueObjects;

public readonly record struct PlanId(Guid Value) : IStronglyTypedId<PlanId>
{
    public static PlanId Create() => new(Guid.CreateVersion7());
    public static PlanId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
