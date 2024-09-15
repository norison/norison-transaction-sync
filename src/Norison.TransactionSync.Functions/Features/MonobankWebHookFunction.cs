using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;

using Norison.TransactionSync.Functions.Models;
using Norison.TransactionSync.Functions.Options;

namespace Norison.TransactionSync.Functions.Features;

public class MonobankWebHookFunction(ServiceBusClient busClient, IOptions<QueueOptions> options)
{
    [Function(nameof(MonobankWebHookFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "monobank/{chatId}")]
        HttpRequest req,
        CancellationToken cancellationToken)
    {
        if (req.Method == "GET")
        {
            return new OkResult();
        }

        using var reader = new StreamReader(req.Body);

        var @event = new TransactionEvent
        {
            ChatId = long.Parse(req.RouteValues["chatId"]!.ToString()!),
            Data = await reader.ReadToEndAsync(cancellationToken)
        };

        var message = new ServiceBusMessage(JsonSerializer.Serialize(@event));

        await busClient
            .CreateSender(options.Value.TransactionsQueueName)
            .SendMessageAsync(message, cancellationToken);

        return new OkResult();
    }
}