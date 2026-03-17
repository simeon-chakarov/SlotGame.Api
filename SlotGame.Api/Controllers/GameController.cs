using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlotGame.Api.DTOs;
using SlotGame.Api.Services;

namespace SlotGame.Api.Controllers;

[ApiController]
[Route("api")]
public class GamesController(IGameService gameService, IValidator<CreateGameRequest> createGameRequestValidator) : ApiControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IValidator<CreateGameRequest> _createGameRequestValidator = createGameRequestValidator;

    /// <summary>
    /// Creates a new game configuration with the specified reel strips and stores it in the database.
    /// </summary>
    /// <param name="request">The game name and symbol strips for each of the 8 reels.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created game's <c>id</c> and <c>name</c>.</returns>
    [HttpPost("game")]
    public async Task<ActionResult<CreateGameResponse>> CreateGame([FromBody] CreateGameRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _createGameRequestValidator.ValidateAsync(request, cancellationToken);
        if (ValidationProblemOrNull(validationResult) is ActionResult validationResponse)
        {
            return validationResponse;
        }

        var response = await _gameService.CreateGameAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetGameById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Returns a list of all games, containing only their <c>id</c> and <c>name</c>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("games")]
    public async Task<ActionResult<List<GameListItemResponse>>> GetGames(CancellationToken cancellationToken)
    {
        var games = await _gameService.GetGamesAsync(cancellationToken);
        return Ok(games);
    }

    /// <summary>
    /// Returns a single game by its <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The game identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The game's <c>id</c> and <c>name</c>, or 404 if not found.</returns>
    [HttpGet("games/{id:int}")]
    public async Task<ActionResult<GameListItemResponse>> GetGameById(int id, CancellationToken cancellationToken)
    {
        var game = await _gameService.GetGameByIdAsync(id, cancellationToken);

        if (game is null)
        {
            return NotFound();
        }

        return Ok(game);
    }
}