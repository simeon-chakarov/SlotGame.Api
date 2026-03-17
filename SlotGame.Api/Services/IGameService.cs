using SlotGame.Api.DTOs;

namespace SlotGame.Api.Services;

public interface IGameService
{
    Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request, CancellationToken cancellationToken = default);

    Task<List<GameListItemResponse>> GetGamesAsync(CancellationToken cancellationToken = default);

    Task<GameListItemResponse?> GetGameByIdAsync(int id, CancellationToken cancellationToken = default);
}