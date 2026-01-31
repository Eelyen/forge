using Forge.Api.Domain.Abstractions;
using Forge.Api.Domain.Constraints;
using Forge.Api.Domain.Exceptions;
using Forge.Api.Domain.Guards;
using Forge.Api.Domain.Text;
using Forge.Api.Domain.ValueObjects;

namespace Forge.Api.Domain.Entities;

public class Recipe : Entity<RecipeId>
{
    private Recipe() { } // EF Core

    public string Slug { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public decimal CycleSeconds { get; private set; }

    private readonly List<RecipeLine> _ingredients = [];
    private readonly List<RecipeLine> _products = [];
    private readonly List<BuildingId> _producedIn = [];

    public IReadOnlyCollection<RecipeLine> Ingredients => _ingredients;
    public IReadOnlyCollection<RecipeLine> Products => _products;
    public IReadOnlyCollection<BuildingId> ProducedIn => _producedIn;

    public static Recipe Create(
        string slug,
        string name,
        decimal cycleSeconds,
        IEnumerable<RecipeLine> ingredients,
        IEnumerable<RecipeLine> products,
        IEnumerable<BuildingId>? producedIn = null,
        string? description = null,
        RecipeId? id = null)
    {
        var recipe = new Recipe
        {
            Id = id ?? RecipeId.New(),
            Name = Guard.Required(name, nameof(name), RecipeConstraints.NameMaxLength),
            Description = Guard.Optional(description, nameof(description), RecipeConstraints.DescriptionMaxLength),
            CycleSeconds = Guard.InRange(cycleSeconds, nameof(cycleSeconds), RecipeConstraints.MinCycleSeconds, RecipeConstraints.MaxCycleSeconds)
        };

        recipe.ChangeSlug(slug);
        recipe.ReplaceIngredients(ingredients);
        recipe.ReplaceProducts(products);

        if (producedIn is not null)
            recipe.ReplaceProducedIn(producedIn);

        recipe.ValidateInvariants();
        return recipe;
    }

    public void Rename(string name) =>
        Name = Guard.Required(name, nameof(name), RecipeConstraints.NameMaxLength);

    public void ChangeSlug(string slug) =>
        Slug = Guard.Required(Slugify.Normalize(slug), nameof(slug), RecipeConstraints.SlugMaxLength);

    public void ChangeDescription(string? description) =>
        Description = Guard.Optional(description, nameof(description), RecipeConstraints.DescriptionMaxLength);

    public void ChangeCycleSeconds(decimal cycleSeconds) =>
        CycleSeconds = Guard.InRange(cycleSeconds, nameof(cycleSeconds), RecipeConstraints.MinCycleSeconds, RecipeConstraints.MaxCycleSeconds);

    public void ReplaceIngredients(IEnumerable<RecipeLine> ingredients) =>
        ReplaceLines(_ingredients, ingredients, nameof(ingredients));

    public void ReplaceProducts(IEnumerable<RecipeLine> products) =>
        ReplaceLines(_products, products, nameof(products));

    public void ReplaceProducedIn(IEnumerable<BuildingId> producedIn)
    {
        Guard.NotNull(producedIn, nameof(producedIn));

        _producedIn.Clear();
        foreach (var b in producedIn)
        {
            Guard.NotDefault(b, nameof(producedIn));
            _producedIn.Add(b);
        }
    }

    private static void ReplaceLines(List<RecipeLine> target, IEnumerable<RecipeLine> source, string parameterName)
    {
        Guard.NotNull(source, parameterName);

        var validated = source.ToList();
        foreach (var line in validated)
        {
            Guard.NotDefault(line.ItemId, $"{parameterName}.{nameof(RecipeLine.ItemId)}");
            Guard.Positive(line.AmountPerCycle, $"{parameterName}.{nameof(RecipeLine.AmountPerCycle)}");
        }

        var consolidated = source
            .GroupBy(l => l.ItemId)
            .Select(g => new RecipeLine(g.Key, g.Sum(x => x.AmountPerCycle)))
            .OrderBy(l => l.ItemId.Value)
            .ToList();

        if (consolidated.Count == 0)
            throw new DomainException($"{parameterName} must contain at least one line.");

        if (target.Count == consolidated.Count && target.SequenceEqual(consolidated))
            return;

        target.Clear();
        target.AddRange(consolidated);
    }

    private void ValidateInvariants()
    {
        if (_ingredients.Count == 0)
            throw new DomainException("A recipe must have at least one ingredient.");
        if (_products.Count == 0)
            throw new DomainException("A recipe must have at least one product.");
    }
}