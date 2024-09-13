using AutoFixture;

using FluentAssertions;

using Monobank.Client;

using Norison.TransactionSync.Application.Features.Commands.Disable;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Storages.Models;

using NSubstitute;

namespace Norison.TransactionSync.Application.Tests.Features.Commands.Disable;

public class DisableCommandHandlerTests
{
    private readonly Fixture _fixture;
    private readonly IUsersService _usersService;
    private readonly IMonobankClient _monobankClient;
    private readonly DisableCommandHandler _sut;

    public DisableCommandHandlerTests()
    {
        _fixture = new Fixture();
        _usersService = Substitute.For<IUsersService>();
        _monobankClient = Substitute.For<IMonobankClient>();
        _sut = new DisableCommandHandler(_usersService, _monobankClient);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThenThrowsInvalidOperationException()
    {
        // Arrange
        var request = _fixture.Create<DisableCommand>();
        var cancellationToken = _fixture.Create<CancellationToken>();
        _usersService.GetUserByChatIdAsync(request.ChatId, cancellationToken).Returns((UserDbModel?)null);

        // Act & Assert
        await _sut
            .Invoking(async x => await x.Handle(request, cancellationToken))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task Handle_WhenUserFound_ThenSetWebHook()
    {
        // Arrange
        var request = _fixture.Create<DisableCommand>();
        var user = _fixture.Create<UserDbModel>();
        var cancellationToken = _fixture.Create<CancellationToken>();
        _usersService.GetUserByChatIdAsync(request.ChatId, cancellationToken).Returns(user);

        // Act
        await _sut.Handle(request, cancellationToken);

        // Assert
        await _monobankClient.Personal.Received(1).SetWebHookAsync("", user.MonoToken, cancellationToken);
    }
}