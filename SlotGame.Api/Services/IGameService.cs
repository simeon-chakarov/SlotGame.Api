using SlotGame.Api.DTOs;

namespace SlotGame.Api.Services;

public interface IGameService
{
    /// <summary>
    /// Creates a new game with the provided name and reel strip configuration and persists it.
    /// </summary>
    Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all games as a lightweight list containing only <c>id</c> and <c>name</c>.
    /// </summary>
    Task<List<GameListItemResponse>> GetGamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single game by ID.
    /// </summary>
    /// <returns><see langword="null"/> if no game with the given ID exists.</returns>
    Task<GameListItemResponse?> GetGameByIdAsync(int id, CancellationToken cancellationToken = default);
}