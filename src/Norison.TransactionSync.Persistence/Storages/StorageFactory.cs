using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Norison.TransactionSync.Persistence.Options;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages;

public class StorageFactory(IMemoryCache memoryCache, IOptions<StorageFactoryOptions> options) : IStorageFactory
{
    public IStorage<UserDbModel> GetUsersStorage()
    {
        return memoryCache.GetOrCreate("UsersStorage",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = options.Value.NotionToken });
                return new Storage<UserDbModel>(client, memoryCache, "Users");
            })!;
    }

    public IStorage<AccountDbModel> GetAccountsStorage(string token)
    {
        return memoryCache.GetOrCreate($"AccountsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<AccountDbModel>(client, memoryCache, "Accounts");
            })!;
    }

    public IStorage<TransactionDbModel> GetTransactionsStorage(string token)
    {
        return memoryCache.GetOrCreate($"TransactionsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<TransactionDbModel>(client, memoryCache, "Transactions");
            })!;
    }

    public IStorage<BudgetDbModel> GetBudgetsStorage(string token)
    {
        return memoryCache.GetOrCreate($"BudgetsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<BudgetDbModel>(client, memoryCache, "Budgets");
            })!;
    }
}