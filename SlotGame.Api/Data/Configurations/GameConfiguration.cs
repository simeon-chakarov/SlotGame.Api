using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlotGame.Api.Entities;

namespace SlotGame.Api.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
    }
}
