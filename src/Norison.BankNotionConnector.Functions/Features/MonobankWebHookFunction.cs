using System.Text.Json;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

using Monobank.Client;

using Norison.BankNotionConnector.Application.Features.Commands.ProcessMonoWebHookData;

namespace Norison.BankNotionConnector.Functions.Features;

public class MonobankWebHookFunction(ISender sender)
{
    [Function(nameof(MonobankWebHookFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "monobank/{username}")]
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

        var command = new ProcessMonoWebHookDataCommand { WebHookData = body.Data };

        await sender.Send(command, cancellationToken);

        return new OkResult();
    }
}