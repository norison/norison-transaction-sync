using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Monobank.Client;

using Norison.TransactionSync.Functions.Models;
using Norison.TransactionSync.Functions.Options;

namespace Norison.TransactionSync.Functions.Features;

public class MonobankWebHookFunction(
    ServiceBusClient busClient,
    IMemoryCache memoryCache,
    IOptions<QueueOptions> options)
{
    [Function(nameof(MonobankWebHookFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "monobank/{chatId}")]
        HttpRequest req)
    {
        if (req.Method == "GET")
        {
            return new OkResult();
        }

        using var reader = new StreamReader(req.Body);
        var content = await reader.ReadToEndAsync();
        var webHookModel = JsonSerializer.Deserialize<WebHookModel>(content);

        if (webHookModel is null || memoryCache.TryGetValue(webHookModel.Data.StatementItem.Id, out _))
        {
            return new OkResult();
        }

        var chatId = long.Parse(req.RouteValues["chatId"]!.ToString()!);
        var data = webHookModel.Data;
        var transactionId = data.StatementItem.Id;

        var @event = new TransactionEvent { ChatId = chatId, Data = data };

        await using var sender = busClient.CreateSender(options.Value.TransactionsQueueName);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(@event));
        await sender.SendMessageAsync(message);

        memoryCache.Set(transactionId, transactionId, TimeSpan.FromMinutes(10));

        return new OkResult();
    }
}