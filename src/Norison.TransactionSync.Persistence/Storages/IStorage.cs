using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages;

public interface IStorage<T>
{
    Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken = default);

    Task<T[]> GetAllAsync(
        string databaseId, DatabasesQueryParameters? parameters = null, CancellationToken cancellationToken = default);

    Task<T?> GetFirstAsync(
        string databaseId, DatabasesQueryParameters? parameters = null, CancellationToken cancellationToken = default);

    Task AddAsync(string databaseId, T item, CancellationToken cancellationToken = default);
    Task UpdateAsync(string databaseId, T item, CancellationToken cancellationToken = default);
}