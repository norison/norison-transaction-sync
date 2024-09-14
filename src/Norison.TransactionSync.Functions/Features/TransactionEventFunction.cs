using System.Text.Json;

using MediatR;

using Microsoft.Azure.Functions.Worker;

using Norison.TransactionSync.Application.Features.Commands.ProcessMonoWebHookData;
using Norison.TransactionSync.Functions.Models;

namespace Norison.TransactionSync.Functions.Features;

public class TransactionEventFunction(ISender sender)
{
    [Function(nameof(TransactionEventFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger("transactionsqueue", Connection = "ServiceBusConnectionString")]
        string message, CancellationToken cancellationToken)
    {
        var @event = JsonSerializer.Deserialize<TransactionEvent>(message);

        if (@event is null)
        {
            return;
        }

        var command = new ProcessMonoWebHookDataCommand { ChatId = @event.ChatId, WebHookData = @event.WebHookData };

        await sender.Send(command, cancellationToken);
    }
}
