using Forge.Api.Domain.Abstractions;
using Forge.Api.Domain.Constraints;
using Forge.Api.Domain.Guards;
using Forge.Api.Domain.Text;
using Forge.Api.Domain.ValueObjects;

namespace Forge.Api.Domain.Entities;

public class Plan : Entity<PlanId>
{
    private Plan() { } // EF Core

    public string Slug { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    private readonly List<PlanTarget> _targets = [];
    private readonly List<PlanInput> _availableInputs = [];

    public IReadOnlyCollection<PlanTarget> Targets => _targets;    
    public IReadOnlyCollection<PlanInput> AvailableInputs => _availableInputs;

    public static Plan Create(string name, string slug)
    {
        var plan = new Plan
        {
            Id = PlanId.Create(),
            Name = Guard.Required(name, nameof(name), PlanConstraints.NameMaxLength)            
        };

        plan.ChangeSlug(slug);
        return plan;
    }

    public void Rename(string name) =>
        Name = Guard.Required(name, nameof(name), PlanConstraints.NameMaxLength);

    public void ChangeSlug(string slug) =>
        Slug = Guard.Required(Slugify.Normalize(slug), nameof(slug), PlanConstraints.SlugMaxLength);

    public void ReplaceTargets(IEnumerable<PlanTarget> targets)
    { 
        Guard.NotNull(targets, nameof(targets));

        var consolidated = targets
            .GroupBy(t => t.ItemId)
            .Select(g => new PlanTarget(g.Key, g.Sum(x => x.AmountPerCycle)))
            .ToList();

        if (_targets.SequenceEqual(consolidated))
            return;

        _targets.Clear();
        _targets.AddRange(consolidated);
    }

    public void ReplaceAvailableInputs(IEnumerable<PlanInput> inputs)
    {
        Guard.NotNull(inputs, nameof(inputs));

        var consolidated = inputs
            .GroupBy(t => t.ItemId)
            .Select(g => new PlanInput(g.Key, g.Sum(x => x.AmountPerCycle)))
            .ToList();

        if (_availableInputs.SequenceEqual(consolidated))
            return;

        _availableInputs.Clear();
        _availableInputs.AddRange(consolidated);
    }
}
