using MediatR;

using Microsoft.Extensions.Options;

using Monobank.Client;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Persistence.Storages;

using Notion.Client;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Features.Enable;

public class EnableCommandHandler(
    IStorageFactory storageFactory,
    ITelegramBotClient client,
    IMonobankClient monobankClient,
    IOptions<WebHookOptions> options) : IRequestHandler<EnableCommand>
{
    public async Task Handle(EnableCommand request, CancellationToken cancellationToken)
    {
        var userStorage = storageFactory.GetUsersStorage();

        var parameters = new DatabasesQueryParameters { Filter = new NumberFilter("ChatId", request.ChatId) };
        var user = await userStorage.GetFirstAsync(parameters, cancellationToken);

        if (user is null)
        {
            await client.SendTextMessageAsync(request.ChatId, "Settings were not found. Use /setsettings to set them.",
                cancellationToken: cancellationToken);
            return;
        }
        
        var url = options.Value.WebHookBaseUrl + $"/monobank/{request.ChatId}";

        await monobankClient.Personal.SetWebHookAsync(url, user.MonoToken, cancellationToken);
        
        await client.SendTextMessageAsync(request.ChatId, "Enable command executed successfully.",
            cancellationToken: cancellationToken);
    }
}