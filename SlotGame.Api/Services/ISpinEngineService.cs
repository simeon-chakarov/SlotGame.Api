using SlotGame.Api.DTOs;

namespace SlotGame.Api.Services;

public interface ISpinEngineService
{
    Task<SpinResponse?> ExecuteSpinAsync(SpinRequest request, CancellationToken cancellationToken = default);

    Task<GetSpinResponse?> GetSpinByIdAsync(int spinId, CancellationToken cancellationToken = default);

    Task<List<HistoryItemResponse>> GetHistoryAsync(int spinsPerPage, int pageNumber, CancellationToken cancellationToken = default);
}