namespace Forge.Api.Domain.Constraints;

public static class RecipeConstraints
{
    public const int SlugMaxLength = 120;
    public const int NameMaxLength = 120;
    public const int DescriptionMaxLength = 500;

    public const decimal MinCycleSeconds = 0.01m;
    public const decimal MaxCycleSeconds = 3600m;
}
