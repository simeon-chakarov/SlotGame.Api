namespace SlotGame.Api.DTOs;

public class SpinResponse
{
    public int SpinId { get; set; }

    public decimal TotalWin { get; set; }

    public List<SpinStateResponse> States { get; set; } = [];
}