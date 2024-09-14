using System.Text.Encodings.Web;
using System.Text.Json;

using Microsoft.Extensions.Options;

using Monobank.Client;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

namespace Norison.TransactionSync.Application.Services.Journal;

public class JournalService(IStorageFactory storageFactory, IOptions<NotionOptions> options) : IJournalService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true,
    };

    public async Task LogTransactionAsync(
        string username, Statement statement, TransactionDbModel transaction,
        CancellationToken cancellationToken = default)
    {
        var storage = storageFactory.GetJournalsStorage();

        var log = new JournalDbModel
        {
            Username = username,
            Statement = JsonSerializer.Serialize(statement, _jsonSerializerOptions),
            Transaction = JsonSerializer.Serialize(transaction, _jsonSerializerOptions)
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
            Statement = JsonSerializer.Serialize(statement, _jsonSerializerOptions),
            Error = exception.ToString()
        };

        await storage.AddAsync(options.Value.NotionJournalsDatabaseId, log, cancellationToken);
    }
}