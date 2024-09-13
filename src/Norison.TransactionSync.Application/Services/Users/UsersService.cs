using Microsoft.Extensions.Options;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

namespace Norison.TransactionSync.Application.Services.Users;

public class UsersService(IStorageFactory storageFactory, IOptions<NotionOptions> options) : IUsersService
{
    public async Task<UserDbModel?> GetUserByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var storage = storageFactory.GetUsersStorage();

        var usersDatabaseId = options.Value.NotionUsersDatabaseId;

        var parameters = new DatabasesQueryParameters { Filter = new NumberFilter(nameof(UserDbModel.ChatId), chatId) };
        return await storage.GetFirstAsync(usersDatabaseId, parameters, cancellationToken);
    }

    public async Task SetUserAsync(UserDbModel user, CancellationToken cancellationToken = default)
    {
        var storage = storageFactory.GetUsersStorage();

        var usersDatabaseId = options.Value.NotionUsersDatabaseId;

        var existingUser = await GetUserByChatIdAsync(user.ChatId, cancellationToken);

        if (existingUser is null)
        {
            await storage.AddAsync(usersDatabaseId, user, cancellationToken);
        }
        else
        {
            user.Id = existingUser.Id;
            await storage.UpdateAsync(usersDatabaseId, user, cancellationToken);
        }
    }
}