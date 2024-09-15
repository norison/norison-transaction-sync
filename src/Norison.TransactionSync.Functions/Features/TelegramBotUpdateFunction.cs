using Mediator;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

using Norison.TransactionSync.Application.Features.Notifications.TelegramUpdate;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Norison.TransactionSync.Functions.Features;

public class TelegramBotUpdateFunction(IPublisher publisher)
{
    [Function(nameof(TelegramBotUpdateFunction))]
    public async Task RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "bot")]
        HttpRequest req, CancellationToken cancellationToken)
    {
        var update = await req.ReadFromJsonAsync<Update>(JsonBotAPI.Options, cancellationToken);

        if (update is null)
        {
            return;
        }

        await publisher.Publish(new TelegramUpdateNotification { Update = update }, cancellationToken);
    }
}