namespace SlotGame.Api.DTOs;

public class HistoryResponse
{
    public List<HistoryItemResponse> Items { get; set; } = [];

    public int TotalCount { get; set; }

    public int PageNumber { get; set; }

    public int SpinsPerPage { get; set; }

    public int TotalPages { get; set; }
}
