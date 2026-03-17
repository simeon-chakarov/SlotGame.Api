using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlotGame.Api.DTOs;
using SlotGame.Api.Services;

namespace SlotGame.Api.Controllers;

[ApiController]
[Route("api")]
public class GamesController(IGameService gameService, IValidator<CreateGameRequest> createGameRequestValidator) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IValidator<CreateGameRequest> _createGameRequestValidator = createGameRequestValidator;

    [HttpPost("game")]
    public async Task<ActionResult<CreateGameResponse>> CreateGame([FromBody] CreateGameRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _createGameRequestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => new
                {
                    Field = x.PropertyName,
                    Message = x.ErrorMessage
                })
                .ToList();

            return BadRequest(new { Errors = errors });
        }

        var response = await _gameService.CreateGameAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetGameById), new { id = response.Id }, response);
    }

    [HttpGet("games")]
    public async Task<ActionResult<List<GameListItemResponse>>> GetGames(CancellationToken cancellationToken)
    {
        var games = await _gameService.GetGamesAsync(cancellationToken);
        return Ok(games);
    }

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