namespace SlotGame.Api.Entities;

public class ReelSymbol
{
    public int Id { get; set; }

    public int ReelStripId { get; set; }

    public int Position { get; set; }

    public int Value { get; set; }

    public ReelStrip ReelStrip { get; set; } = null!;
}
