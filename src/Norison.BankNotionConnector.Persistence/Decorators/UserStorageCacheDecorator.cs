using Microsoft.Extensions.Caching.Memory;

using Norison.BankNotionConnector.Persistence.Storages.Users;
using Norison.BankNotionConnector.Persistence.Storages.Users.Models;

namespace Norison.BankNotionConnector.Persistence.Decorators;

public class UserStorageCacheDecorator(IUsersStorage usersStorage, IMemoryCache memoryCache) : IUsersStorage
{
    public async Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken)
    {
        return (await memoryCache.GetOrCreateAsync("UserStorage_GetDatabaseIdAsync",
            async _ => await usersStorage.GetDatabaseIdAsync(cancellationToken)))!;
    }

    public async Task<UserModel[]> GetUsersAsync(CancellationToken cancellationToken)
    {
        return (await memoryCache.GetOrCreateAsync("UserStorage_GetUsersAsync",
            async _ => await usersStorage.GetUsersAsync(cancellationToken)))!;
    }

    public async Task<UserModel?> GetUserAsync(string username, CancellationToken cancellationToken)
    {
        return (await memoryCache.GetOrCreateAsync($"UserStorage_GetUserAsync_{username}",
            async _ => await usersStorage.GetUserAsync(username, cancellationToken)))!;
    }

    public async Task AddUserAsync(UserModel user, CancellationToken cancellationToken)
    {
        await usersStorage.AddUserAsync(user, cancellationToken);
        memoryCache.Remove("UserStorage_GetUsersAsync");
        memoryCache.Remove($"UserStorage_GetUserAsync_{user.Username}");
    }

    public async Task UpdateUserAsync(UserModel user, CancellationToken cancellationToken)
    {
        await usersStorage.UpdateUserAsync(user, cancellationToken);
        memoryCache.Remove("UserStorage_GetUsersAsync");
        memoryCache.Remove($"UserStorage_GetUserAsync_{user.Username}");
    }
}