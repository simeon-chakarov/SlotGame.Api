namespace SlotGame.Api.Services;

/// <summary>
/// Production implementation of <see cref="IRandomProvider"/> backed by <see cref="Random.Shared"/>.
/// </summary>
public sealed class RandomProvider : IRandomProvider
{
    /// <inheritdoc/>
    public int Next(int maxValue) => Random.Shared.Next(maxValue);
}
