using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages;

public abstract class StorageBase(INotionClient client)
{
    protected async Task<Database> GetDatabaseAsync(string databaseName, CancellationToken cancellationToken)
    {
        var parameters = new SearchParameters
        {
            Query = databaseName, Filter = new SearchFilter { Value = SearchObjectType.Database }
        };

        var databases = await client.Search.SearchAsync(parameters, cancellationToken);

        var database = databases.Results.FirstOrDefault();

        if (database is null)
        {
            throw new InvalidOperationException($"Database '{databaseName}' not found.");
        }

        return (Database)database;
    }
}