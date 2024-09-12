using Norison.BankNotionConnector.Persistence.Storages.Models;

namespace Norison.BankNotionConnector.Persistence.Storages;

public interface IStorageFactory
{
    IStorage<UserDbModel> GetUsersStorage();
    IStorage<AccountDbModel> GetAccountsStorage(string token);
    IStorage<TransactionDbModel> GetTransactionsStorage(string token);
    IStorage<BudgetDbModel> GetBudgetsStorage(string token);
}