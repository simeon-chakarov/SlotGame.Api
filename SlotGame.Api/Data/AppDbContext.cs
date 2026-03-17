using Microsoft.EntityFrameworkCore;
using SlotGame.Api.Entities;

namespace SlotGame.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<ReelStrip> ReelStrips => Set<ReelStrip>();
    public DbSet<ReelSymbol> ReelSymbols => Set<ReelSymbol>();
    public DbSet<Spin> Spins => Set<Spin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
