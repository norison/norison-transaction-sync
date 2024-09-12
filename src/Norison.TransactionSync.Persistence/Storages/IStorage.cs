using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages;

public interface IStorage<T>
{
    Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken);
    Task<T?> GetFirstAsync(string databaseId, DatabasesQueryParameters parameters, CancellationToken cancellationToken);
    Task AddAsync(string databaseId, T item, CancellationToken cancellationToken);
    Task UpdateAsync(string databaseId, T item, CancellationToken cancellationToken);
}