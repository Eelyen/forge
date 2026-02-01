using Forge.Api.Domain.Abstractions;
using Forge.Api.Domain.Constraints;
using Forge.Api.Domain.Entities;
using Forge.Api.Domain.ValueObjects;
using Forge.Api.Infrastructure.Data.ValueConverters;

using Microsoft.EntityFrameworkCore;

namespace Forge.Api.Infrastructure.Data;

public class ForgeDbContext : DbContext
{
    public ForgeDbContext(DbContextOptions<ForgeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Plan> Plans => Set<Plan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureItem(modelBuilder);
        ConfigureBuilding(modelBuilder);
        ConfigureRecipe(modelBuilder);
        ConfigurePlan(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAudit();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAudit();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAudit()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.SetCreated(now);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.Touch(now);
            }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Find all record structs in the Domain that implement IStronglyTypedId
        var idTypes = typeof(ItemId).Assembly.GetTypes()
            .Where(t => t.IsValueType && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStronglyTypedId<>)));

        foreach (var idType in idTypes)
        {
            // Dynamically create the converter type for this specific ID
            var converterType = typeof(SqlSequentialIdConverter<>).MakeGenericType(idType);
            configurationBuilder.Properties(idType).HaveConversion(converterType);
        }
    }

    private static void ConfigureItem(ModelBuilder modelBuilder)
    {
        var b = modelBuilder.Entity<Item>();

        b.ToTable("Items");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Slug).IsRequired().HasMaxLength(ItemConstraints.SlugMaxLength);
        b.Property(x => x.Name).IsRequired().HasMaxLength(ItemConstraints.NameMaxLength);
        b.Property(x => x.Description).HasMaxLength(ItemConstraints.DescriptionMaxLength);
        b.Property(x => x.CreatedUtc).IsRequired();
        b.Property(x => x.UpdatedUtc).IsRequired();
        b.HasIndex(x => x.Slug).IsUnique();
    }

    private static void ConfigureBuilding(ModelBuilder modelBuilder)
    {
        var b = modelBuilder.Entity<Building>();
        b.ToTable("Buildings");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Slug).IsRequired().HasMaxLength(BuildingConstraints.SlugMaxLength);
        b.Property(x => x.Name).IsRequired().HasMaxLength(BuildingConstraints.NameMaxLength);
        b.Property(x => x.Description).HasMaxLength(BuildingConstraints.DescriptionMaxLength);
        b.Property(x => x.CreatedUtc).IsRequired();
        b.Property(x => x.UpdatedUtc).IsRequired();
        b.HasIndex(x => x.Slug).IsUnique();
    }

    private static void ConfigureRecipe(ModelBuilder modelBuilder)
    {
        var b = modelBuilder.Entity<Recipe>();
        b.ToTable("Recipes");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Slug).IsRequired().HasMaxLength(RecipeConstraints.SlugMaxLength);
        b.Property(x => x.Name).IsRequired().HasMaxLength(RecipeConstraints.NameMaxLength);
        b.Property(x => x.Description).HasMaxLength(RecipeConstraints.DescriptionMaxLength);
        b.Property(x => x.CycleSeconds).HasPrecision(18, 3);
        b.Property(x => x.CreatedUtc).IsRequired();
        b.Property(x => x.UpdatedUtc).IsRequired();
        b.HasIndex(x => x.Slug).IsUnique();

        b.Navigation(x => x.Ingredients).UsePropertyAccessMode(PropertyAccessMode.Field);
        b.Navigation(x => x.Products).UsePropertyAccessMode(PropertyAccessMode.Field);
        b.Navigation(x => x.ProducedIn).UsePropertyAccessMode(PropertyAccessMode.Field);

        b.OwnsMany(x => x.Ingredients, owned =>
        {
            owned.ToTable("RecipeIngredients");
            owned.WithOwner().HasForeignKey(nameof(RecipeId));
            owned.Property(x => x.AmountPerCycle).HasPrecision(18, 3);
            owned.HasKey(nameof(RecipeId), nameof(RecipeLine.ItemId));
        });

        b.OwnsMany(x => x.Products, owned =>
        {
            owned.ToTable("RecipeProducts");
            owned.WithOwner().HasForeignKey(nameof(RecipeId));
            owned.Property(x => x.AmountPerCycle).HasPrecision(18, 3);
            owned.HasKey(nameof(RecipeId), nameof(RecipeLine.ItemId));
        });

        b.OwnsMany("_producedIn", nameof(Recipe.ProducedIn), owned =>
        {
            owned.ToTable("RecipeProducedIn");
            owned.WithOwner().HasForeignKey(nameof(RecipeId));
            owned.HasKey(nameof(RecipeId), nameof(BuildingId));
        });
    }

    private static void ConfigurePlan(ModelBuilder modelBuilder)
    {
        var b = modelBuilder.Entity<Plan>();
        b.ToTable("Plans");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Slug).IsRequired().HasMaxLength(PlanConstraints.SlugMaxLength);
        b.Property(x => x.Name).IsRequired().HasMaxLength(PlanConstraints.NameMaxLength);
        b.Property(x => x.CreatedUtc).IsRequired();
        b.Property(x => x.UpdatedUtc).IsRequired();
        b.HasIndex(x => x.Slug).IsUnique();

        b.Navigation(x => x.Targets).UsePropertyAccessMode(PropertyAccessMode.Field);
        b.Navigation(x => x.AvailableInputs).UsePropertyAccessMode(PropertyAccessMode.Field);

        b.OwnsMany(x => x.Targets, owned =>
        {
            owned.ToTable("PlanTargets");
            owned.WithOwner().HasForeignKey(nameof(PlanId));
            owned.Property(x => x.AmountPerCycle).HasPrecision(18, 3);
            owned.HasKey(nameof(PlanId), nameof(PlanTarget.ItemId));
        });

        b.OwnsMany(x => x.AvailableInputs, owned =>
        {
            owned.ToTable("PlanInputs");
            owned.WithOwner().HasForeignKey(nameof(PlanId));
            owned.Property(x => x.AmountPerCycle).HasPrecision(18, 3);
            owned.HasKey(nameof(PlanId), nameof(PlanInput.ItemId));
        });
    }
}
