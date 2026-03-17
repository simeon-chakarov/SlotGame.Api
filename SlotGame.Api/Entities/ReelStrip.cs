namespace SlotGame.Api.Entities;

public class ReelStrip
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int ColumnIndex { get; set; }

    public Game Game { get; set; } = null!;

    public List<ReelSymbol> Symbols { get; set; } = [];
}
