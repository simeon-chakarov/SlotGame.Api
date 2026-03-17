using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlotGame.Api.Constants;
using SlotGame.Api.Data;

namespace SlotGame.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController(AppDbContext dbContext) : ControllerBase
{
    private readonly AppDbContext _dbContext = dbContext;

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(SuccessMessages.ApiIsWorking);
    }

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
