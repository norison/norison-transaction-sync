using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages;

public interface IStorage<T>
{
    Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken);
    Task<T[]> GetAllAsync(DatabasesQueryParameters parameters, CancellationToken cancellationToken);
    Task<T?> GetFirstAsync(DatabasesQueryParameters parameters, CancellationToken cancellationToken);
    Task AddAsync(T item, CancellationToken cancellationToken);
    Task UpdateAsync(T item, CancellationToken cancellationToken);
}