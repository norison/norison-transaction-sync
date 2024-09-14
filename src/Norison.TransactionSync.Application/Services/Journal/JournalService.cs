using System.Text.Json;

using Microsoft.Extensions.Options;

using Monobank.Client;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

namespace Norison.TransactionSync.Application.Services.Journal;

public class JournalService(IStorageFactory storageFactory, IOptions<NotionOptions> options) : IJournalService
{
    public async Task LogTransactionAsync(
        string username, Statement statement, TransactionDbModel transaction,
        CancellationToken cancellationToken = default)
    {
        var storage = storageFactory.GetJournalsStorage();

        var log = new JournalDbModel
        {
            Username = username,
            Statement = JsonSerializer.Serialize(statement),
            Transaction = JsonSerializer.Serialize(transaction)
        };

        await storage.AddAsync(options.Value.NotionJournalsDatabaseId, log, cancellationToken);
    }

    public async Task LogTransactionErrorAsync(
        string username, Statement statement, Exception exception, CancellationToken cancellationToken = default)
    {
        var storage = storageFactory.GetJournalsStorage();

        var log = new JournalDbModel
        {
            Username = username,
            Statement = JsonSerializer.Serialize(statement),
            Error = exception.ToString()
        };

        await storage.AddAsync(options.Value.NotionJournalsDatabaseId, log, cancellationToken);
    }
}