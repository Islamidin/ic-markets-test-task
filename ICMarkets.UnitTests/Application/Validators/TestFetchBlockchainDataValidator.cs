using FluentValidation.TestHelper;
using ICMarkets.Application.Commands.FetchBlockchainData;
using ICMarkets.Domain.Enums;

namespace ICMarkets.UnitTests.Application.Validators;

[TestFixture]
public class TestFetchBlockchainDataValidator
{
    private FetchBlockchainDataValidator validator;

    [SetUp]
    public void Setup()
    {
        validator = new();
    }

    [Test]
    public void Should_Have_Error_When_Blockchain_Is_Invalid()
    {
        var model = new FetchBlockchainDataCommand((BlockchainType) 999);
        var result = validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Blockchain);
    }

    [Test]
    public void Should_Not_Have_Error_When_Blockchain_Is_Valid()
    {
        var model = new FetchBlockchainDataCommand(BlockchainType.Btc);
        var result = validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Blockchain);
    }
}