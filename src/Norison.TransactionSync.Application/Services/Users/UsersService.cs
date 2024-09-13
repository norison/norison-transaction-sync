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
}