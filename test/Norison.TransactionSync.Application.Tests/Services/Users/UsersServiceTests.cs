using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.Options;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

using NSubstitute;

using PAP.NSubstitute.FluentAssertionsBridge;

namespace Norison.TransactionSync.Application.Tests.Services.Users;

public class UsersServiceTests
{
    private readonly Fixture _fixture;
    private readonly IStorage<UserDbModel> _userStorage;
    private readonly UsersService _sut;

    public UsersServiceTests()
    {
        var storageFactory = Substitute.For<IStorageFactory>();
        var options = Substitute.For<IOptions<NotionOptions>>();

        _fixture = new Fixture();
        _userStorage = Substitute.For<IStorage<UserDbModel>>();
        _sut = new UsersService(storageFactory, options);

        options.Value.Returns(new NotionOptions { NotionUsersDatabaseId = "usersDatabaseId" });

        storageFactory.GetUsersStorage().Returns(_userStorage);
    }

    [Fact]
    public async Task GetUserByChatIdAsync_WhenUserNotFound_ThenReturnsNull()
    {
        // Arrange
        var chatId = _fixture.Create<long>();
        var cancellationToken = _fixture.Create<CancellationToken>();

        // Act
        var result = await _sut.GetUserByChatIdAsync(chatId, cancellationToken);

        // Assert
        result.Should().BeNull();

        await _userStorage
            .Received(1)
            .GetFirstAsync("usersDatabaseId", Verify.That<DatabasesQueryParameters>(parameters =>
            {
                var numberFilter = parameters.Filter.Should().BeOfType<NumberFilter>().Subject;

                numberFilter.Property.Should().Be(nameof(UserDbModel.ChatId));
                numberFilter.Number.Equal.Should().Be(chatId);
            }), cancellationToken);
    }

    [Fact]
    public async Task SetUserAsync_WhenUserNotFound_ThenAddsUser()
    {
        // Arrange
        var user = _fixture.Create<UserDbModel>();
        var cancellationToken = _fixture.Create<CancellationToken>();

        _userStorage
            .GetFirstAsync("usersDatabaseId", Arg.Any<DatabasesQueryParameters>(), cancellationToken)
            .Returns((UserDbModel?)null);

        // Act
        await _sut.SetUserAsync(user, cancellationToken);

        // Assert
        await _userStorage.Received(1).AddAsync("usersDatabaseId", user, cancellationToken);
    }

    [Fact]
    public async Task SetUserAsync_WhenUserFound_ThenUpdatesUser()
    {
        // Arrange
        var user = _fixture.Create<UserDbModel>();
        var existingUser = _fixture.Create<UserDbModel>();
        var cancellationToken = _fixture.Create<CancellationToken>();

        _userStorage
            .GetFirstAsync("usersDatabaseId", Arg.Any<DatabasesQueryParameters>(), cancellationToken)
            .Returns(existingUser);

        // Act
        await _sut.SetUserAsync(user, cancellationToken);

        // Assert
        await _userStorage.Received(1).UpdateAsync("usersDatabaseId", user, cancellationToken);
    }
}