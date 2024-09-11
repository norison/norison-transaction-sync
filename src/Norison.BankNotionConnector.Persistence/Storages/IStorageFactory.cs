using Norison.BankNotionConnector.Persistence.Storages.Accounts;
using Norison.BankNotionConnector.Persistence.Storages.Users;

namespace Norison.BankNotionConnector.Persistence.Storages;

public interface IStorageFactory
{
    IUsersStorage GetUsersStorage();
    IAccountsStorage GetAccountsStorage(string token);
}