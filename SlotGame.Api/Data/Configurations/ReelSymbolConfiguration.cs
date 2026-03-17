using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlotGame.Api.Entities;

namespace SlotGame.Api.Data.Configurations;

public class ReelSymbolConfiguration : IEntityTypeConfiguration<ReelSymbol>
{
    public void Configure(EntityTypeBuilder<ReelSymbol> builder)
    {
        builder.HasOne(x => x.ReelStrip)
            .WithMany(x => x.Symbols)
            .HasForeignKey(x => x.ReelStripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ReelStripId, x.Position })
            .IsUnique();
    }
}
