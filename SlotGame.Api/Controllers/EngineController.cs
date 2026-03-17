using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlotGame.Api.Constants;
using SlotGame.Api.DTOs;
using SlotGame.Api.Services;

namespace SlotGame.Api.Controllers;

/// <summary>
/// Handles spin execution and spin retrieval. Route: <c>api/engine</c>.
/// </summary>
[ApiController]
[Route("api/engine")]
public class EngineController(ISpinEngineService spinEngineService, IValidator<SpinRequest> spinRequestValidator) : ApiControllerBase
{
    private readonly ISpinEngineService _spinEngineService = spinEngineService;
    private readonly IValidator<SpinRequest> _spinRequestValidator = spinRequestValidator;

    /// <summary>
    /// Executes a spin for the specified game: generates an 8×8 matrix, evaluates wins,
    /// runs up to 10 cascade (tumbling) steps, and persists the final result.
    /// </summary>
    /// <param name="request">The game ID and bet amount for this spin.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All matrix states (initial + each cascade) and the combined total win.</returns>
    [HttpPost]
    public async Task<ActionResult<SpinResponse>> ExecuteSpin([FromBody] SpinRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _spinRequestValidator.ValidateAsync(request, cancellationToken);
        if (ValidationProblemOrNull(validationResult) is ActionResult validationResponse)
        {
            return validationResponse;
        }

        var response = await _spinEngineService.ExecuteSpinAsync(request, cancellationToken);

        if (response is null)
        {
            return NotFound(new { error = ErrorMessages.GameNotFound });
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a previously executed spin by its ID.
    /// </summary>
    /// <param name="spinId">The spin identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The saved spin details including its final matrix state and total win.</returns>
    [HttpGet]
    public async Task<ActionResult<GetSpinResponse>> GetSpin([FromQuery] int spinId, CancellationToken cancellationToken)
    {
        if (spinId <= 0)
        {
            return BadRequest(new { message = ErrorMessages.SpinIdMustBeGreaterThanZero });
        }

        var response = await _spinEngineService.GetSpinByIdAsync(spinId, cancellationToken);

        if (response is null)
        {
            return NotFound(new { error = ErrorMessages.SpinNotFound });
        }

        return Ok(response);
    }
}