namespace SlotGame.Api.Entities;

public class Game
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public List<ReelStrip> ReelStrips { get; set; } = [];

    public List<Spin> Spins { get; set; } = [];
}
