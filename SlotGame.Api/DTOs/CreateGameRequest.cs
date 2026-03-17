namespace SlotGame.Api.DTOs;

public class CreateGameRequest
{
    public string Name { get; set; } = null!;

    public List<List<int>> SymbolsPerReel { get; set; } = [];
}
