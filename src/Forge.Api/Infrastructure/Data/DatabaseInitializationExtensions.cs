namespace Forge.Api.Infrastructure.Data;

internal static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ForgeDbContext>();

        // I'll add migrations a little later
        await db.Database.EnsureCreatedAsync(cancellationToken);

        await ForgeDbSeeder.SeedAsync(db, cancellationToken);
    }
}
