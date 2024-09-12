using MediatR;

using Norison.TransactionSync.Persistence.Storages;

using Notion.Client;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Features.GetSettings;

public class GetSettingsCommandHandler(IStorageFactory storageFactory, ITelegramBotClient client)
    : IRequestHandler<GetSettingsCommand>
{
    public async Task Handle(GetSettingsCommand request, CancellationToken cancellationToken)
    {
        var usersStorage = storageFactory.GetUsersStorage();

        var parameters = new DatabasesQueryParameters { Filter = new TitleFilter("Username", request.Username) };
        var user = await usersStorage.GetFirstAsync(parameters, cancellationToken);

        if (user is null)
        {
            await client.SendTextMessageAsync(request.ChatId, "User not found", cancellationToken: cancellationToken);
            return;
        }

        var message = "Your settings:\n" +
                      $"Notion token: {user.NotionToken}\n" +
                      $"Mono token: {user.MonoToken}\n" +
                      $"Mono account name: {user.MonoAccountName}";

        await client.SendTextMessageAsync(request.ChatId, message, cancellationToken: cancellationToken);
    }
}