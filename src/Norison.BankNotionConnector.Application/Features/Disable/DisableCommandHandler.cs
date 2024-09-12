using MediatR;

using Monobank.Client;

using Norison.BankNotionConnector.Persistence.Storages;

using Notion.Client;

using Telegram.Bot;

namespace Norison.BankNotionConnector.Application.Features.Disable;

public class DisableCommandHandler(
    IStorageFactory storageFactory,
    ITelegramBotClient client,
    IMonobankClient monobankClient) : IRequestHandler<DisableCommand>
{
    public async Task Handle(DisableCommand request, CancellationToken cancellationToken)
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

        await monobankClient.Personal.SetWebHookAsync("", user.MonoToken, cancellationToken);

        await client.SendTextMessageAsync(request.ChatId, "Disable command executed successfully.",
            cancellationToken: cancellationToken);
    }
}