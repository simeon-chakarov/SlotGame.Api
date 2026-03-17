namespace SlotGame.Api.Services.Models;

public class WinEvaluationResult
{
    public HashSet<int> WinningSymbols { get; set; } = [];

    public decimal CascadeWin { get; set; }
}