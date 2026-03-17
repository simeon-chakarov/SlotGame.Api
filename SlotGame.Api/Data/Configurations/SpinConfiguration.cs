using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlotGame.Api.Entities;

namespace SlotGame.Api.Data.Configurations;

public class SpinConfiguration : IEntityTypeConfiguration<Spin>
{
    public void Configure(EntityTypeBuilder<Spin> builder)
    {
        builder.HasOne(x => x.Game)
            .WithMany(x => x.Spins)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.BetAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalWin)
            .HasPrecision(18, 2);

        builder.Property(x => x.FinalMatrixJson)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql(DatabaseDefaults.UtcNow);
    }
}