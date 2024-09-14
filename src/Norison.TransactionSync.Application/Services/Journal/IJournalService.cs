using Monobank.Client;

using Norison.TransactionSync.Persistence.Storages.Models;

namespace Norison.TransactionSync.Application.Services.Journal;

public interface IJournalService
{
    Task LogTransactionAsync(
        string username, Statement statement, TransactionDbModel transaction,
        CancellationToken cancellationToken = default);

    Task LogTransactionErrorAsync(
        string username, Statement statement, Exception exception,
        CancellationToken cancellationToken = default);
}