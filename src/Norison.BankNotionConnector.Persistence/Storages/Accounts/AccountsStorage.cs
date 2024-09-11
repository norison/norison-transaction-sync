using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages.Accounts;

public class AccountsStorage(INotionClient client) : StorageBase(client), IAccountsStorage
{
    private const string DatabaseName = "Accounts";

    public async Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken)
    {
        var database = await GetDatabaseAsync(DatabaseName, cancellationToken);
        return database.Id;
    }
}