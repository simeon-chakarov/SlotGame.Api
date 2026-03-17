namespace SlotGame.Api.Entities;

public class Spin
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public decimal BetAmount { get; set; }

    public decimal TotalWin { get; set; }

    public string FinalMatrixJson { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public Game Game { get; set; } = null!;
}
