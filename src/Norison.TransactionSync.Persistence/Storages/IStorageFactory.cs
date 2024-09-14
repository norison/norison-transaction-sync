using Norison.TransactionSync.Persistence.Storages.Models;

namespace Norison.TransactionSync.Persistence.Storages;

public interface IStorageFactory
{
    IStorage<UserDbModel> GetUsersStorage();
    IStorage<JournalDbModel> GetJournalsStorage();
    IStorage<AccountDbModel> GetAccountsStorage(string token);
    IStorage<TransactionDbModel> GetTransactionsStorage(string token);
    IStorage<BudgetDbModel> GetBudgetsStorage(string token);
    IStorage<CategoryDbModel> GetCategoriesStorage(string token);
    IStorage<CurrencyDbModel> GetCurrenciesStorage(string token);
    IStorage<AutomationsDbModel> GetAutomationsStorage(string token);
}