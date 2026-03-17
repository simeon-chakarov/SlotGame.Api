using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlotGame.Api.Constants;
using SlotGame.Api.DTOs;
using SlotGame.Api.Services;

namespace SlotGame.Api.Controllers;

[ApiController]
[Route("api/engine")]
public class EngineController(ISpinEngineService spinEngineService, IValidator<SpinRequest> spinRequestValidator) : ControllerBase
{
    private readonly ISpinEngineService _spinEngineService = spinEngineService;
    private readonly IValidator<SpinRequest> _spinRequestValidator = spinRequestValidator;

    [HttpPost]
    public async Task<ActionResult<SpinResponse>> ExecuteSpin([FromBody] SpinRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _spinRequestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => new
                {
                    Field = x.PropertyName,
                    Message = x.ErrorMessage
                })
                .ToList();

            return BadRequest(new 
            { 
                Errors = errors 
            });
        }

        var response = await _spinEngineService.ExecuteSpinAsync(request, cancellationToken);

        if (response is null)
        {
            return NotFound(new
            {
                error = ErrorMessages.GameNotFound
            });
        }

        return Ok(response);
    }

    [HttpGet("{spinId:int}")]
    public async Task<ActionResult<GetSpinResponse>> GetSpin(int spinId, CancellationToken cancellationToken)
    {
        var response = await _spinEngineService.GetSpinByIdAsync(spinId, cancellationToken);

        if (response is null)
        {
            return NotFound(new
            {
                error = ErrorMessages.SpinNotFound
            });
        }

        return Ok(response);
    }
}