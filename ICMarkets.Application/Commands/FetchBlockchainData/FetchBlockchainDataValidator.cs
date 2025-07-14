using FluentValidation;

namespace ICMarkets.Application.Commands.FetchBlockchainData;

public class FetchBlockchainDataValidator : AbstractValidator<FetchBlockchainDataCommand>
{
    public FetchBlockchainDataValidator()
    {
        RuleFor(x => x.Blockchain)
            .IsInEnum()
            .WithMessage("Invalid blockchain type.");
    }
}