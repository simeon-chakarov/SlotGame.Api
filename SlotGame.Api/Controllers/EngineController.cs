using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlotGame.Api.Constants;
using SlotGame.Api.DTOs;
using SlotGame.Api.Services;

namespace SlotGame.Api.Controllers;

[ApiController]
[Route("api/engine")]
public class EngineController(ISpinEngineService spinEngineService, IValidator<SpinRequest> spinRequestValidator) : ApiControllerBase
{
    private readonly ISpinEngineService _spinEngineService = spinEngineService;
    private readonly IValidator<SpinRequest> _spinRequestValidator = spinRequestValidator;

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