using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.Options;

using Monobank.Client;

using Norison.TransactionSync.Application.Features.Commands.Enable;
using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Storages.Models;

using NSubstitute;

namespace Norison.TransactionSync.Application.Tests.Features.Commands.Enable;

public class EnableCommandHandlerTests
{
    private readonly Fixture _fixture;
    private readonly IUsersService _usersService;
    private readonly IMonobankClient _monobankClient;
    private readonly IOptions<WebHookOptions> _webHookOptions;
    private readonly EnableCommandHandler _sut;

    public EnableCommandHandlerTests()
    {
        _fixture = new Fixture();
        _usersService = Substitute.For<IUsersService>();
        _monobankClient = Substitute.For<IMonobankClient>();
        _webHookOptions = Substitute.For<IOptions<WebHookOptions>>();
        _sut = new EnableCommandHandler(_usersService, _monobankClient, _webHookOptions);

        _webHookOptions.Value.Returns(new WebHookOptions { WebHookBaseUrl = "https://example.com" });
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThenThrowsInvalidOperationException()
    {
        // Arrange
        var request = _fixture.Create<EnableCommand>();
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
        var request = _fixture.Create<EnableCommand>();
        var user = _fixture.Create<UserDbModel>();
        var cancellationToken = _fixture.Create<CancellationToken>();
        _usersService.GetUserByChatIdAsync(request.ChatId, cancellationToken).Returns(user);
        var url = $"{_webHookOptions.Value.WebHookBaseUrl}/monobank/{request.ChatId}";

        // Act
        await _sut.Handle(request, cancellationToken);

        // Assert
        await _monobankClient.Personal.Received(1).SetWebHookAsync(url, user.MonoToken, cancellationToken);
    }
}