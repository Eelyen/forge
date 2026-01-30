using Forge.Api.Domain;

using Microsoft.EntityFrameworkCore;

namespace Forge.Api.Infrastructure.Data;

public class ForgeDbContext : DbContext
{
    public ForgeDbContext(DbContextOptions<ForgeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();
}
