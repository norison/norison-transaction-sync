namespace Norison.BankNotionConnector.Persistence.Storages;

public interface IStorage
{
    Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken);
}