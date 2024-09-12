using Norison.BankNotionConnector.Persistence.Storages.Accounts;
using Norison.BankNotionConnector.Persistence.Storages.Budgets;
using Norison.BankNotionConnector.Persistence.Storages.Transactions;
using Norison.BankNotionConnector.Persistence.Storages.Users;

namespace Norison.BankNotionConnector.Persistence.Storages;

public interface IStorageFactory
{
    IStorage<UserDbModel> GetUsersStorage();
    IStorage<AccountDbModel> GetAccountsStorage(string token);
    IStorage<TransactionDbModel> GetTransactionsStorage(string token);
    IStorage<BudgetDbModel> GetBudgetsStorage(string token);
}