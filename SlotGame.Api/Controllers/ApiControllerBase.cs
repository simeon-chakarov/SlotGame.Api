using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace SlotGame.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
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