using System.Text.Json;

namespace SlotGame.Api.Helpers;

public static class MatrixSerializer
{
    public static string Serialize(int[][] matrix)
    {
        return JsonSerializer.Serialize(matrix);
    }

    public static int[][] Deserialize(string json)
    {
        return JsonSerializer.Deserialize<int[][]>(json) ?? [];
    }
}