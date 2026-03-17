using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace SlotGame.Api.Controllers;

/// <summary>
/// Base controller that provides shared helpers for all API controllers.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Returns a 400 Bad Request with field-level error details if validation failed,
    /// or <see langword="null"/> if the result is valid.
    /// </summary>
    protected ActionResult? ValidationProblemOrNull(ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return null;
        }

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
}