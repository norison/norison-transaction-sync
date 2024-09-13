using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

using Monobank.Client;

using Norison.TransactionSync.Functions.Models;

namespace Norison.TransactionSync.Functions.Features;

public class MonobankWebHookFunction(ServiceBusClient busClient)
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

        var bodyString = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

        var body = JsonSerializer.Deserialize<WebHookModel>(bodyString);

        if (body is null)
        {
            return new BadRequestResult();
        }

        var chatId = long.Parse(req.RouteValues["chatId"]!.ToString()!);

        var @event = new TransactionEvent { ChatId = chatId, WebHookData = body.Data };

        var sender = busClient.CreateSender("transactionsqueue");
        var message = new ServiceBusMessage(JsonSerializer.Serialize(@event));
        await sender.SendMessageAsync(message, cancellationToken);

        return new OkResult();
    }
}