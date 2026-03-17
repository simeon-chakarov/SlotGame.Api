using FluentValidation;
using SlotGame.Api.Constants;
using SlotGame.Api.DTOs;

namespace SlotGame.Api.Validators;

public class SpinRequestValidator : AbstractValidator<SpinRequest>
{
    public SpinRequestValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0)
            .WithMessage(ErrorMessages.GameIdMustBeGreaterThanZero);

        RuleFor(x => x.BetAmount)
            .GreaterThan(0)
            .WithMessage(ErrorMessages.BetAmountMustBeGreaterThanZero);
    }
}