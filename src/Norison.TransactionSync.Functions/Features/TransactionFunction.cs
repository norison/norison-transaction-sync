using System.Text.Json;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

using Norison.TransactionSync.Application.Features.Commands.ProcessMonoWebHookData;
using Norison.TransactionSync.Functions.Models;

namespace Norison.TransactionSync.Functions.Features;

public class TransactionFunction(ISender sender)
{
    [Function(nameof(TransactionFunction))]
    public async Task RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "transaction")]
        HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var content = await new StreamReader(httpRequest.Body).ReadToEndAsync(cancellationToken);
        var @event = JsonSerializer.Deserialize<TransactionEvent>(content);

        if (@event is null)
        {
            return;
        }

        var command = new ProcessMonoWebHookDataCommand { ChatId = @event.ChatId, WebHookData = @event.WebHookData };

        await sender.Send(command, cancellationToken);
    }
}