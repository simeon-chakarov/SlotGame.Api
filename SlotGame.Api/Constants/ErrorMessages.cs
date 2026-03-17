namespace SlotGame.Api.Constants;

public static class ErrorMessages
{
    public const string ConnectionStringNotFound = "Connection string 'DefaultConnection' not found.";
    public const string UnhandledExceptionMessage = "An unhandled exception occurred.";

    public const string GameNameRequired = "Game name is required.";
    public const string GameNameTooLong = "Game name must be 200 characters or fewer.";
    public const string SymbolsPerReelRequired = "Symbols per reel are required.";
    public const string ExactlyEightReelsRequired = "Exactly 8 reels are required.";
    public const string ReelMustContainSymbols = "Reel at index {CollectionIndex} must contain at least one symbol.";
    public const string SymbolValueOutOfRange = "Symbol values must be between 0 and 255 inclusive.";

    public const string GameIdMustBeGreaterThanZero = "GameId must be greater than 0.";
    public const string BetAmountMustBeGreaterThanZero = "BetAmount must be greater than 0.";
    public const string GameNotFound = "Game not found.";
    public const string SpinNotFound = "Spin not found.";
    public const string SpinIdMustBeGreaterThanZero = "spinId must be greater than 0.";

    public const string SpinPerPageMustBeGreaterThanZero = "spinsPerPage must be greater than 0.";
    public const string PageNumberMustBeGreaterThanZero = "pageNumber must be greater than 0.";
}