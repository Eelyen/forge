using Forge.Api.Domain.Entities;
using Forge.Api.Domain.Enums;
using Forge.Api.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace Forge.Api.Infrastructure.Data;

internal static class ForgeDbSeeder
{
    public static async Task SeedAsync(ForgeDbContext db, CancellationToken cancellationToken = default)
    {
        // Idempotent seed: skip if we already have our canonical seed item(s).
        // (Slug check is more stable than Any().)
        if (await db.Items.AnyAsync(x => x.Slug == "iron-ore", cancellationToken))
            return;

        // ---- Buildings
        var smelter = Building.Create("smelter", "Smelter");
        var constructor = Building.Create("constructor", "Constructor");

        // ---- Items
        var ironOre = Item.Create("iron-ore", "Iron Ore", UnitKind.Item, isRawResource: true);
        var ironIngot = Item.Create("iron-ingot", "Iron Ingot", UnitKind.Item);
        var ironPlate = Item.Create("iron-plate", "Iron Plate", UnitKind.Item);
        var ironRod = Item.Create("iron-rod", "Iron Rod", UnitKind.Item);
        var screw = Item.Create("screw", "Screw", UnitKind.Item);

        // ---- Recipes (simple toy chain, great for testing plan math + graphing later)
        var smeltIron = Recipe.Create(
            slug: "smelt-iron-ore",
            name: "Smelt Iron Ore",
            cycleSeconds: 1.0m,
            ingredients: [new RecipeLine(ironOre.Id, 1m)],
            products: [new RecipeLine(ironIngot.Id, 1m)],
            producedIn: [smelter.Id]);

        var makeIronPlate = Recipe.Create(
            slug: "iron-plate",
            name: "Iron Plate",
            cycleSeconds: 3.0m,
            ingredients: [new RecipeLine(ironIngot.Id, 2m)],
            products: [new RecipeLine(ironPlate.Id, 1m)],
            producedIn: [constructor.Id]);

        var makeIronRod = Recipe.Create(
            slug: "iron-rod",
            name: "Iron Rod",
            cycleSeconds: 4.0m,
            ingredients: [new RecipeLine(ironIngot.Id, 1m)],
            products: [new RecipeLine(ironRod.Id, 1m)],
            producedIn: [constructor.Id]);

        var makeScrew = Recipe.Create(
            slug: "screw",
            name: "Screw",
            cycleSeconds: 1.5m,
            ingredients: [new RecipeLine(ironRod.Id, 1m)],
            products: [new RecipeLine(screw.Id, 4m)],
            producedIn: [constructor.Id]);

        // ---- Plan
        var starterPlan = Plan.Create("Starter Plan", "starter-plan");

        starterPlan.ReplaceTargets(
        [
            new PlanTarget(ironPlate.Id, 10m),
            new PlanTarget(screw.Id, 40m)
        ]);

        starterPlan.ReplaceAvailableInputs(
        [
            new PlanInput(ironOre.Id, 120m)
        ]);

        db.AddRange(smelter, constructor);
        db.AddRange(ironOre, ironIngot, ironPlate, ironRod, screw);
        db.AddRange(smeltIron, makeIronPlate, makeIronRod, makeScrew);
        db.Add(starterPlan);

        await db.SaveChangesAsync(cancellationToken);
    }
}