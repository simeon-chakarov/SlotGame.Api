using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlotGame.Api.Entities;

namespace SlotGame.Api.Data.Configurations;

public class ReelStripConfiguration : IEntityTypeConfiguration<ReelStrip>
{
    public void Configure(EntityTypeBuilder<ReelStrip> builder)
    {
        builder.HasOne(x => x.Game)
            .WithMany(x => x.ReelStrips)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.GameId, x.ColumnIndex })
            .IsUnique();
    }
}
