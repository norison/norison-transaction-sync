using System.Text.Json;

using Mediator;

using Microsoft.Azure.Functions.Worker;

using Monobank.Client;

using Norison.TransactionSync.Application.Features.Commands.ProcessMonoWebHookData;
using Norison.TransactionSync.Functions.Models;

namespace Norison.TransactionSync.Functions.Features;

public class TransactionEventFunction(ISender sender)
{
    [Function(nameof(TransactionEventFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger("%TransactionsQueueName%", Connection = "ServiceBusConnectionString")]
        string message, CancellationToken cancellationToken)
    {
        var @event = JsonSerializer.Deserialize<TransactionEvent>(message);

        if (@event is null)
        {
            return;
        }

        var webHookData = JsonSerializer.Deserialize<WebHookModel>(@event.Data);

        if (webHookData is null)
        {
            return;
        }

        var command = new ProcessMonoWebHookDataCommand { ChatId = @event.ChatId, WebHookData = webHookData.Data };
        await sender.Send(command, cancellationToken);
    }
}