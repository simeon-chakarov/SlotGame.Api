namespace SlotGame.Api.DTOs;

public class HistoryItemResponse
{
    public int SpinId { get; set; }

    public int GameId { get; set; }

    public decimal BetAmount { get; set; }

    public decimal TotalWin { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public int[][] FinalMatrix { get; set; } = [];
}
