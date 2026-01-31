namespace Forge.Api.Domain.ValueObjects;

public readonly record struct RecipeId(Guid Value)
{
    public static RecipeId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
