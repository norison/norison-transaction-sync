using System.Text.Json;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

using Monobank.Client;

using Norison.TransactionSync.Application.Features.ProcessMonoWebHookData;

namespace Norison.TransactionSync.Functions.Features;

public class MonobankWebHookFunction(ISender sender)
{
    [Function(nameof(MonobankWebHookFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "monobank/{chatId}")]
        HttpRequest req, CancellationToken cancellationToken)
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

        var command = new ProcessMonoWebHookDataCommand { ChatId = chatId, WebHookData = body.Data };

        await sender.Send(command, cancellationToken);

        return new OkResult();
    }
}