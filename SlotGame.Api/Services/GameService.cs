using Microsoft.EntityFrameworkCore;
using SlotGame.Api.Data;
using SlotGame.Api.DTOs;
using SlotGame.Api.Entities;

namespace SlotGame.Api.Services;

public class GameService(AppDbContext dbContext) : IGameService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<CreateGameResponse> CreateGameAsync(
        CreateGameRequest request,
        CancellationToken cancellationToken = default)
    {
        var game = new Game
        {
            Name = request.Name.Trim()
        };

        for (int columnIndex = 0; columnIndex < request.SymbolsPerReel.Count; columnIndex++)
        {
            var reelSymbols = request.SymbolsPerReel[columnIndex];

            var reelStrip = new ReelStrip
            {
                ColumnIndex = columnIndex
            };

            for (int position = 0; position < reelSymbols.Count; position++)
            {
                reelStrip.Symbols.Add(new ReelSymbol
                {
                    Position = position,
                    Value = reelSymbols[position]
                });
            }

            game.ReelStrips.Add(reelStrip);
        }

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateGameResponse
        {
            Id = game.Id,
            Name = game.Name
        };
    }

    public async Task<List<GameListItemResponse>> GetGamesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new GameListItemResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<GameListItemResponse?> GetGameByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new GameListItemResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}