namespace SlotGame.Api.DTOs;

public class SpinStateResponse
{
    public int CascadeNumber { get; set; }

    public int[][] Matrix { get; set; } = [];

    public decimal CascadeWin { get; set; }
}
