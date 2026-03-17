using Microsoft.AspNetCore.Mvc;
using SlotGame.Api.Constants;
using SlotGame.Api.DTOs;
using SlotGame.Api.Services;

namespace SlotGame.Api.Controllers;

[ApiController]
[Route("api/history")]
public class HistoryController(ISpinEngineService spinEngineService) : ControllerBase
{
    private readonly ISpinEngineService _spinEngineService = spinEngineService;

    private const int DefaultPageNumber = 1;
    private const int DefaultSpinsPerPage = 10;
    private const int MaxSpinsPerPage = 100;
    

    /// <summary>
    /// Returns a paginated list of all spins, ordered by ID ascending.
    /// </summary>
    /// <param name="spinsPerPage">Number of results per page (default 10, max 100).</param>
    /// <param name="pageNumber">1-based page number (default 1).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    public async Task<ActionResult<List<HistoryItemResponse>>> GetHistory(
        [FromQuery] int spinsPerPage = DefaultSpinsPerPage,
        [FromQuery] int pageNumber = DefaultPageNumber,
        CancellationToken cancellationToken = default)
    {
        if (spinsPerPage <= 0)
        {
            return BadRequest(new
            {
                error = ErrorMessages.SpinPerPageMustBeGreaterThanZero
            });
        }

        if (pageNumber <= 0)
        {
            return BadRequest(new
            {
                error = ErrorMessages.PageNumberMustBeGreaterThanZero
            });
        }

        if (spinsPerPage > MaxSpinsPerPage)
        {
            spinsPerPage = MaxSpinsPerPage;
        }

        var history = await _spinEngineService.GetHistoryAsync(
            spinsPerPage,
            pageNumber,
            cancellationToken);

        return Ok(history);
    }
}