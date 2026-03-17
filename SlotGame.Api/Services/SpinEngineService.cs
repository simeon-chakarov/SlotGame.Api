using Microsoft.EntityFrameworkCore;
using SlotGame.Api.Constants;
using SlotGame.Api.Data;
using SlotGame.Api.DTOs;
using SlotGame.Api.Entities;
using SlotGame.Api.Helpers;
using SlotGame.Api.Services.Models;

namespace SlotGame.Api.Services;

public class SpinEngineService(AppDbContext dbContext) : ISpinEngineService
{
    private const int MatrixSize = 8;
    private const int MaxCascades = 10;

    private readonly AppDbContext _dbContext = dbContext;
    private readonly Random _random = new();

    /// <inheritdoc/>
    public async Task<SpinResponse?> ExecuteSpinAsync(SpinRequest request, CancellationToken cancellationToken = default)
    {
        var game = await _dbContext.Games
            .Include(x => x.ReelStrips)
            .ThenInclude(x => x.Symbols)
            .FirstOrDefaultAsync(x => x.Id == request.GameId, cancellationToken);

        if (game is null)
        {
            return null;
        }

        var orderedReels = game.ReelStrips
            .OrderBy(x => x.ColumnIndex)
            .ToList();

        if (orderedReels.Count != MatrixSize)
        {
            throw new InvalidOperationException(ErrorMessages.ExactlyEightReelsRequired);
        }

        var reelValues = orderedReels
            .Select(r => r.Symbols
                .OrderBy(s => s.Position)
                .Select(s => s.Value)
                .ToList())
            .ToList();

        var initialMatrixResult = GenerateInitialMatrix(reelValues);

        var states = new List<SpinStateResponse>();
        var totalWin = 0m;

        var currentMatrix = CloneMatrix(initialMatrixResult.Matrix);
        var currentTopIndexes = (int[])initialMatrixResult.TopIndexes.Clone();

        // state 0 = initial matrix
        states.Add(new SpinStateResponse
        {
            CascadeNumber = 0,
            Matrix = CloneMatrix(currentMatrix),
            CascadeWin = 0m
        });

        for (int cascadeNumber = 1; cascadeNumber <= MaxCascades; cascadeNumber++)
        {
            var stepResult = ExecuteCascadeStep(
                currentMatrix,
                currentTopIndexes,
                reelValues,
                request.BetAmount);

            if (!stepResult.HasWin)
            {
                break;
            }

            totalWin += stepResult.CascadeWin;
            currentMatrix = CloneMatrix(stepResult.Matrix);

            states.Add(new SpinStateResponse
            {
                CascadeNumber = cascadeNumber,
                Matrix = CloneMatrix(stepResult.Matrix),
                CascadeWin = stepResult.CascadeWin
            });
        }

        var spin = new Spin
        {
            GameId = request.GameId,
            BetAmount = request.BetAmount,
            TotalWin = totalWin,
            FinalMatrixJson = MatrixSerializer.Serialize(currentMatrix),
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Spins.Add(spin);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SpinResponse
        {
            SpinId = spin.Id,
            TotalWin = totalWin,
            States = states
        };
    }

    /// <inheritdoc/>
    public async Task<GetSpinResponse?> GetSpinByIdAsync(int spinId, CancellationToken cancellationToken = default)
    {
        var spin = await _dbContext.Spins
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == spinId, cancellationToken);

        if (spin is null)
        {
            return null;
        }

        return new GetSpinResponse
        {
            SpinId = spin.Id,
            GameId = spin.GameId,
            BetAmount = spin.BetAmount,
            TotalWin = spin.TotalWin,
            FinalMatrix = MatrixSerializer.Deserialize(spin.FinalMatrixJson),
            CreatedAtUtc = spin.CreatedAtUtc
        };
    }

    /// <inheritdoc/>
    public async Task<List<HistoryItemResponse>> GetHistoryAsync(int spinsPerPage, int pageNumber, CancellationToken cancellationToken = default)
    {
        var skip = (pageNumber - 1) * spinsPerPage;

        var spins = await _dbContext.Spins
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip(skip)
            .Take(spinsPerPage)
            .ToListAsync(cancellationToken);

        return [.. spins
            .Select(x => new HistoryItemResponse
            {
                SpinId = x.Id,
                GameId = x.GameId,
                BetAmount = x.BetAmount,
                TotalWin = x.TotalWin,
                CreatedAtUtc = x.CreatedAtUtc,
                FinalMatrix = MatrixSerializer.Deserialize(x.FinalMatrixJson)
            })];
    }

    private InitialMatrixResult GenerateInitialMatrix(List<List<int>> reelValues)
    {
        var matrix = new int[MatrixSize][];
        for (int row = 0; row < MatrixSize; row++)
        {
            matrix[row] = new int[MatrixSize];
        }

        var topIndexes = new int[MatrixSize];

        for (int column = 0; column < MatrixSize; column++)
        {
            var reel = reelValues[column];
            var reelLength = reel.Count;
            var topIndex = _random.Next(reelLength);

            topIndexes[column] = topIndex;

            for (int row = 0; row < MatrixSize; row++)
            {
                var symbolIndex = (topIndex + row) % reelLength;
                matrix[row][column] = reel[symbolIndex];
            }
        }

        return new InitialMatrixResult
        {
            Matrix = matrix,
            TopIndexes = topIndexes
        };
    }

    private static CascadeStepResult ExecuteCascadeStep(
        int[][] currentMatrix,
        int[] topIndexes,
        List<List<int>> reelValues,
        decimal betAmount)
    {
        var winEvaluation = EvaluateMatrix(currentMatrix, betAmount);

        if (winEvaluation.WinningSymbols.Count == 0)
        {
            return new CascadeStepResult
            {
                Matrix = CloneMatrix(currentMatrix),
                CascadeWin = 0m,
                HasWin = false
            };
        }

        var workingMatrix = ToNullableMatrix(currentMatrix);

        RemoveWinningSymbols(workingMatrix, winEvaluation.WinningSymbols);
        ApplyGravity(workingMatrix);
        RefillMatrix(workingMatrix, topIndexes, reelValues);

        return new CascadeStepResult
        {
            Matrix = ToNonNullableMatrix(workingMatrix),
            CascadeWin = winEvaluation.CascadeWin,
            HasWin = true
        };
    }

    private static WinEvaluationResult EvaluateMatrix(int[][] matrix, decimal betAmount)
    {
        var symbolCounts = CountSymbols(matrix);
        var winningSymbols = GetWinningSymbols(symbolCounts);
        var cascadeWin = CalculateCascadeWin(winningSymbols, betAmount);

        return new WinEvaluationResult
        {
            WinningSymbols = winningSymbols,
            CascadeWin = cascadeWin
        };
    }

    private static Dictionary<int, int> CountSymbols(int[][] matrix)
    {
        var counts = new Dictionary<int, int>();

        for (int row = 0; row < matrix.Length; row++)
        {
            for (int column = 0; column < matrix[row].Length; column++)
            {
                var symbol = matrix[row][column];

                if (!counts.TryAdd(symbol, 1))
                {
                    counts[symbol]++;
                }
            }
        }

        return counts;
    }

    private static HashSet<int> GetWinningSymbols(Dictionary<int, int> symbolCounts)
    {
        return [.. symbolCounts
            .Where(x => x.Key != 0 && x.Value >= 8)
            .Select(x => x.Key)];
    }

    private static decimal CalculateCascadeWin(HashSet<int> winningSymbols, decimal betAmount)
    {
        decimal total = 0m;

        foreach (var symbol in winningSymbols)
        {
            total += symbol * betAmount;
        }

        return total;
    }

    private static int?[][] ToNullableMatrix(int[][] matrix)
    {
        var result = new int?[MatrixSize][];

        for (int row = 0; row < MatrixSize; row++)
        {
            result[row] = new int?[MatrixSize];

            for (int column = 0; column < MatrixSize; column++)
            {
                result[row][column] = matrix[row][column];
            }
        }

        return result;
    }

    private static int[][] ToNonNullableMatrix(int?[][] matrix)
    {
        var result = new int[MatrixSize][];

        for (int row = 0; row < MatrixSize; row++)
        {
            result[row] = new int[MatrixSize];

            for (int column = 0; column < MatrixSize; column++)
            {
                result[row][column] = matrix[row][column] ?? 0;
            }
        }

        return result;
    }

    private static void RemoveWinningSymbols(int?[][] matrix, HashSet<int> winningSymbols)
    {
        for (int row = 0; row < MatrixSize; row++)
        {
            for (int column = 0; column < MatrixSize; column++)
            {
                var value = matrix[row][column];

                if (value.HasValue && winningSymbols.Contains(value.Value))
                {
                    matrix[row][column] = null;
                }
            }
        }
    }

    private static void ApplyGravity(int?[][] matrix)
    {
        for (int column = 0; column < MatrixSize; column++)
        {
            var values = new List<int>();

            for (int row = 0; row < MatrixSize; row++)
            {
                if (matrix[row][column].HasValue)
                {
                    values.Add(matrix[row][column]!.Value);
                }
            }

            var targetRow = MatrixSize - 1;

            for (int i = values.Count - 1; i >= 0; i--)
            {
                matrix[targetRow][column] = values[i];
                targetRow--;
            }

            while (targetRow >= 0)
            {
                matrix[targetRow][column] = null;
                targetRow--;
            }
        }
    }

    private static void RefillMatrix(int?[][] matrix, int[] topIndexes, List<List<int>> reelValues)
    {
        for (int column = 0; column < MatrixSize; column++)
        {
            var emptyCount = 0;

            for (int row = 0; row < MatrixSize; row++)
            {
                if (matrix[row][column] is null)
                {
                    emptyCount++;
                }
                else
                {
                    break;
                }
            }

            if (emptyCount == 0)
            {
                continue;
            }

            var reel = reelValues[column];
            var reelLength = reel.Count;
            var currentTopIndex = topIndexes[column];

            var newValues = new int[emptyCount];

            for (int i = 0; i < emptyCount; i++)
            {
                var symbolIndex = Mod(currentTopIndex - emptyCount + i, reelLength);
                newValues[i] = reel[symbolIndex];
            }

            for (int row = 0; row < emptyCount; row++)
            {
                matrix[row][column] = newValues[row];
            }

            topIndexes[column] = Mod(currentTopIndex - emptyCount, reelLength);
        }
    }

    private static int[][] CloneMatrix(int[][] matrix)
    {
        var clone = new int[matrix.Length][];

        for (int row = 0; row < matrix.Length; row++)
        {
            clone[row] = new int[matrix[row].Length];
            Array.Copy(matrix[row], clone[row], matrix[row].Length);
        }

        return clone;
    }

    private static int Mod(int value, int modulo)
    {
        var result = value % modulo;
        return result < 0 ? result + modulo : result;
    }
}