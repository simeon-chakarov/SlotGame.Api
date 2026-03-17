using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlotGame.Api.Constants;
using SlotGame.Api.Data;

namespace SlotGame.Api.Controllers;

/// <summary>
/// Development-only utilities: health check and database reset. Route: <c>test</c>.
/// </summary>
[ApiController]
[Route("[controller]")]
public class TestController(AppDbContext dbContext) : ControllerBase
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>Health check endpoint.</summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(SuccessMessages.ApiIsWorking);
    }

    /// <summary>
    /// Clears all data from the database. Only available in the Development environment.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("reset")]
    public async Task<IActionResult> ResetDatabase(CancellationToken cancellationToken)
    {
        if (!HttpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .IsDevelopment())
        {
            return NotFound();
        }

        // Order matters (FK constraints)
        await _dbContext.Spins.ExecuteDeleteAsync(cancellationToken);
        await _dbContext.ReelSymbols.ExecuteDeleteAsync(cancellationToken);
        await _dbContext.ReelStrips.ExecuteDeleteAsync(cancellationToken);
        await _dbContext.Games.ExecuteDeleteAsync(cancellationToken);

        return Ok(new { message = SuccessMessages.DatabaseCleared });
    }
}
