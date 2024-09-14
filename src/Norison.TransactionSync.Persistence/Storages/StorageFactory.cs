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
                return new Storage<UserDbModel>(client, "Users");
            })!;
    }

    public IStorage<JournalDbModel> GetJournalsStorage()
    {
        return memoryCache.GetOrCreate("JournalsStorage",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = options.Value.NotionToken });
                return new Storage<JournalDbModel>(client, "Journals");
            })!;
    }

    public IStorage<AccountDbModel> GetAccountsStorage(string token)
    {
        return memoryCache.GetOrCreate($"AccountsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<AccountDbModel>(client, "Accounts");
            })!;
    }

    public IStorage<TransactionDbModel> GetTransactionsStorage(string token)
    {
        return memoryCache.GetOrCreate($"TransactionsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<TransactionDbModel>(client, "Transactions");
            })!;
    }

    public IStorage<BudgetDbModel> GetBudgetsStorage(string token)
    {
        return memoryCache.GetOrCreate($"BudgetsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<BudgetDbModel>(client, "Budgets");
            })!;
    }

    public IStorage<CategoryDbModel> GetCategoriesStorage(string token)
    {
        return memoryCache.GetOrCreate($"CategoriesStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<CategoryDbModel>(client, "Categories");
            })!;
    }

    public IStorage<CurrencyDbModel> GetCurrenciesStorage(string token)
    {
        return memoryCache.GetOrCreate($"CurrenciesStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<CurrencyDbModel>(client, "Currencies");
            })!;
    }

    public IStorage<AutomationsDbModel> GetAutomationsStorage(string token)
    {
        return memoryCache.GetOrCreate($"AutomationsStorage_{token}",
            _ =>
            {
                var client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
                return new Storage<AutomationsDbModel>(client, "Automations");
            })!;
    }
}