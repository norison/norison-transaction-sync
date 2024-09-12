using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages;

public abstract class StorageBase<T>(INotionClient client) : IStorage<T> where T : IDbModel
{
    protected abstract string DatabaseName { get; }

    protected abstract T ConvertPropertiesToModel(string id, IDictionary<string, PropertyValue> properties);
    protected abstract IDictionary<string, PropertyValue> ConvertModelToProperties(T model);

    public async Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken)
    {
        var database = await GetDatabaseAsync(DatabaseName, cancellationToken);
        return database.Id;
    }

    public async Task<T[]> GetAllAsync(DatabasesQueryParameters parameters, CancellationToken cancellationToken)
    {
        var databaseId = await GetDatabaseIdAsync(cancellationToken);
        var pages = await client.Databases.QueryAsync(databaseId, parameters, cancellationToken);
        return pages.Results.Select(x => ConvertPropertiesToModel(x.Id, x.Properties)).ToArray();
    }

    public async Task<T?> GetFirstAsync(DatabasesQueryParameters parameters, CancellationToken cancellationToken)
    {
        var databaseId = await GetDatabaseIdAsync(cancellationToken);
        var pages = await client.Databases.QueryAsync(databaseId, parameters, cancellationToken);
        var page = pages.Results.FirstOrDefault();
        return page is not null ? ConvertPropertiesToModel(page.Id, page.Properties) : default;
    }

    public async Task AddAsync(T item, CancellationToken cancellationToken)
    {
        var databaseId = await GetDatabaseIdAsync(cancellationToken);
        var parameters = new PagesCreateParameters
        {
            Parent = new DatabaseParentInput { DatabaseId = databaseId },
            Properties = ConvertModelToProperties(item)
        };
        await client.Pages.CreateAsync(parameters, cancellationToken);
    }

    public async Task UpdateAsync(T item, CancellationToken cancellationToken)
    {
        var parameters = new PagesUpdateParameters { Properties = ConvertModelToProperties(item) };
        await client.Pages.UpdateAsync(item.Id, parameters, cancellationToken);
    }

    private async Task<Database> GetDatabaseAsync(string databaseName, CancellationToken cancellationToken)
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