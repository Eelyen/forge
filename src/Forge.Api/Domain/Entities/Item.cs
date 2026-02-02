using Forge.Api.Domain.Abstractions;
using Forge.Api.Domain.Constraints;
using Forge.Api.Domain.Enums;
using Forge.Api.Domain.Guards;
using Forge.Api.Domain.Text;
using Forge.Api.Domain.ValueObjects;

namespace Forge.Api.Domain.Entities;

public class Item : Entity<ItemId>
{
    private Item() { } // EF Core

    public string Slug { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public UnitKind UnitKind { get; private set; }
    public bool IsRawResource { get; private set; }

    public static Item Create(        
        string slug,
        string name,
        UnitKind unitKind = UnitKind.Item,
        string? description = null,
        bool isRawResource = false)
    {
        var item = new Item
        {
            Id = ItemId.Create(),
            Name = Guard.Required(name, nameof(name), maxLength: ItemConstraints.NameMaxLength),
            Description = Guard.Optional(description, nameof(description), maxLength: ItemConstraints.DescriptionMaxLength),
            UnitKind = unitKind,
            IsRawResource = isRawResource
        };

        item.ChangeSlug(slug);
        return item;
    }

    public void Rename(string name) =>
        Name = Guard.Required(name, nameof(name), maxLength: ItemConstraints.NameMaxLength);
    public void ChangeSlug(string slug) =>
        Slug = Guard.Required(Slugify.Normalize(slug), nameof(slug), maxLength: ItemConstraints.SlugMaxLength);
    public void ChangeDescription(string? description) =>
        Description = Guard.Optional(description, nameof(description), maxLength: ItemConstraints.DescriptionMaxLength);
    public void ChangeUnitKind(UnitKind unitKind) => UnitKind = unitKind;
    public void SetRawResource(bool isRawResource) => IsRawResource = isRawResource;
}
