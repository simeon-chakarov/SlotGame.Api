using FluentValidation;
using SlotGame.Api.Constants;
using SlotGame.Api.DTOs;

namespace SlotGame.Api.Validators;

public class CreateGameRequestValidator : AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessages.GameNameRequired)
            .MaximumLength(200)
            .WithMessage(ErrorMessages.GameNameTooLong);

        RuleFor(x => x.SymbolsPerReel)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessages.SymbolsPerReelRequired)
            .Must(reels => reels.Count == 8)
            .WithMessage(ErrorMessages.ExactlyEightReelsRequired);

        RuleForEach(x => x.SymbolsPerReel)
            .NotEmpty()
            .WithMessage(ErrorMessages.ReelMustContainSymbols);

        RuleForEach(x => x.SymbolsPerReel)
            .ChildRules(reel =>
            {
                reel.RuleForEach(x => x)
                    .InclusiveBetween(0, 255)
                    .WithMessage(ErrorMessages.SymbolValueOutOfRange);
            });
    }
}