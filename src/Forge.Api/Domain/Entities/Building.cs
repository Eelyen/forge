using Forge.Api.Domain.Abstractions;
using Forge.Api.Domain.Constraints;
using Forge.Api.Domain.Guards;
using Forge.Api.Domain.Text;
using Forge.Api.Domain.ValueObjects;

namespace Forge.Api.Domain.Entities;

public sealed class Building : Entity<BuildingId>
{
    private Building() { } // EF Core

    public string Slug { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public static Building Create(
        string slug,
        string name,
        string? description = null,
        BuildingId? id = null)
    {
        var building = new Building
        {
            Id = id ?? BuildingId.New(),
            Name = Guard.Required(name, nameof(name), BuildingConstraints.NameMaxLength),
            Description = Guard.Optional(description, nameof(description), BuildingConstraints.DescriptionMaxLength)
        };

        building.ChangeSlug(slug);
        return building;
    }

    public void Rename(string name) =>
        Name = Guard.Required(name, nameof(name), BuildingConstraints.NameMaxLength);

    public void ChangeSlug(string slug) =>
        Slug = Guard.Required(Slugify.Normalize(slug), nameof(slug), BuildingConstraints.SlugMaxLength);

    public void ChangeDescription(string? description) =>
        Description = Guard.Optional(description, nameof(description), BuildingConstraints.DescriptionMaxLength);
}
