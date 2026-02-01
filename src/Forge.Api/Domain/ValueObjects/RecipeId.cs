using Forge.Api.Domain.Abstractions;

namespace Forge.Api.Domain.ValueObjects;

public readonly record struct RecipeId(Guid Value) : IStronglyTypedId<RecipeId>
{
    public static RecipeId New() => new(Guid.CreateVersion7());
    public static RecipeId FromGuid(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
