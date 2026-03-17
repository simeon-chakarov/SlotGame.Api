namespace SlotGame.Api.Services;

/// <summary>
/// Abstraction over random number generation, allowing deterministic behaviour in tests.
/// </summary>
public interface IRandomProvider
{
    /// <summary>Returns a non-negative random integer less than <paramref name="maxValue"/>.</summary>
    int Next(int maxValue);
}
