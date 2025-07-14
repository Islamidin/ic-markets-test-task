using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ICMarkets.Application.Behaviors;
using MediatR;
using NSubstitute;

namespace ICMarkets.UnitTests.Application.Behaviors;

[TestFixture]
public class ValidationBehaviorTests
{
    private ValidationBehavior<TestRequest, TestResponse> behavior;
    private CancellationToken cancellationToken;
    private TestRequest request;
    private List<IValidator<TestRequest>> validators;

    [SetUp]
    public void Setup()
    {
        request = new();
        validators = [];
        behavior = new(validators);
        cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        var expectedResponse = new TestResponse { };
        var nextCalled = false;

        var result = await behavior.Handle(request, Next, cancellationToken);

        nextCalled.Should().BeTrue();
        result.Should().BeEquivalentTo(expectedResponse);
        return;

        Task<TestResponse> Next(CancellationToken token)
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        }
    }

    [Test]
    public async Task Handle_ValidatorsPass_ShouldCallNext()
    {
        var expectedResponse = new TestResponse { };
        var nextCalled = false;

        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), cancellationToken)
                 .Returns(new ValidationResult());

        validators.Add(validator);

        var result = await behavior.Handle(request, Next, cancellationToken);

        await validator.Received(1).ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), cancellationToken);
        nextCalled.Should().BeTrue();
        result.Should().BeEquivalentTo(expectedResponse);
        return;

        Task<TestResponse> Next(CancellationToken token)
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        }
    }

    [Test]
    public void Handle_ValidationFails_ShouldThrow()
    {
        var nextCalled = false;

        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), cancellationToken)
                 .Returns(new ValidationResult([
                     new ValidationFailure("Property", "Error message")
                 ]));

        validators.Add(validator);

        Func<Task> act = async () => await behavior.Handle(request, Next, cancellationToken);

        act.Should().ThrowAsync<ValidationException>()
           .WithMessage("Validation failed: \n -- Property: Error message");

        nextCalled.Should().BeFalse();
        return;

        Task<TestResponse> Next(CancellationToken token)
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse());
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public class TestRequest : IRequest<TestResponse>;

    private class TestResponse;
}