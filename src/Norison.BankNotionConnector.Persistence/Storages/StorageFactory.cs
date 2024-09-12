using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Norison.BankNotionConnector.Persistence.Options;
using Norison.BankNotionConnector.Persistence.Storages.Accounts;
using Norison.BankNotionConnector.Persistence.Storages.Budgets;
using Norison.BankNotionConnector.Persistence.Storages.Transactions;
using Norison.BankNotionConnector.Persistence.Storages.Users;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages;

public class StorageFactory(IMemoryCache memoryCache, IOptions<StorageFactoryOptions> options) : IStorageFactory
{
    public IStorage<UserDbModel> GetUsersStorage()
    {
        return memoryCache.GetOrCreate("UsersStorage",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = options.Value.NotionToken });
                return new UsersStorage(client);
            })!;
    }

    public IStorage<AccountDbModel> GetAccountsStorage(string token)
    {
        return memoryCache.GetOrCreate($"AccountsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new AccountsStorage(client);
            })!;
    }

    public IStorage<TransactionDbModel> GetTransactionsStorage(string token)
    {
        return memoryCache.GetOrCreate($"TransactionsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new TransactionsStorage(client);
            })!;
    }

    public IStorage<BudgetDbModel> GetBudgetsStorage(string token)
    {
        return memoryCache.GetOrCreate($"BudgetsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new BudgetsStorage(client);
            })!;
    }
}