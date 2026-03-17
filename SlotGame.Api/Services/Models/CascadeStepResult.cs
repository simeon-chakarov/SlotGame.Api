namespace SlotGame.Api.Services.Models;

public class CascadeStepResult
{
    public int[][] Matrix { get; set; } = [];

    public decimal CascadeWin { get; set; }

    public bool HasWin { get; set; }
}