using SlotGame.Api.DTOs;

namespace SlotGame.Api.Services;

public interface ISpinEngineService
{
    /// <summary>
    /// Runs a full spin: generates the initial 8×8 matrix, cascades until no more wins or the
    /// 10-cascade limit is reached, saves the final state to the database, and returns all
    /// intermediate matrix states together with the total win amount.
    /// </summary>
    /// <returns><see langword="null"/> if the game ID does not exist.</returns>
    Task<SpinResponse?> ExecuteSpinAsync(SpinRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single spin record by ID, including its final matrix state.
    /// </summary>
    /// <returns><see langword="null"/> if no spin with the given ID exists.</returns>
    Task<GetSpinResponse?> GetSpinByIdAsync(int spinId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paginated slice of spin history ordered by ID ascending,
    /// including total count and page metadata.
    /// </summary>
    /// <param name="spinsPerPage">Maximum number of records to return.</param>
    /// <param name="pageNumber">1-based page index.</param>
    Task<HistoryResponse> GetHistoryAsync(int spinsPerPage, int pageNumber, CancellationToken cancellationToken = default);
}