using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Norison.BankNotionConnector.Persistence.Decorators;
using Norison.BankNotionConnector.Persistence.Options;
using Norison.BankNotionConnector.Persistence.Storages.Accounts;
using Norison.BankNotionConnector.Persistence.Storages.Users;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages;

public class StorageFactory(IMemoryCache memoryCache, IOptions<StorageFactoryOptions> options) : IStorageFactory
{
    public IUsersStorage GetUsersStorage()
    {
        return memoryCache.GetOrCreate("UsersStorage",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = options.Value.NotionToken });
                var storage = new UsersStorage(client);
                return new UserStorageCacheDecorator(storage, memoryCache);
            })!;
    }

    public IAccountsStorage GetAccountsStorage(string token)
    {
        return memoryCache.GetOrCreate($"AccountsStorage_{token}",
            _ => new AccountsStorage(
                NotionClientFactory.Create(new ClientOptions { AuthToken = token })))!;
    }
}