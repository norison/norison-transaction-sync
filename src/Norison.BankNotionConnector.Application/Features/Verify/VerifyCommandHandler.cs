using MediatR;

using Monobank.Client;

using Norison.BankNotionConnector.Persistence.Storages;

using Notion.Client;

using Telegram.Bot;

namespace Norison.BankNotionConnector.Application.Features.Verify;

public class VerifyCommandHandler(
    IStorageFactory storageFactory,
    ITelegramBotClient client,
    IMonobankClient monobankClient) : IRequestHandler<VerifyCommand>
{
    public async Task Handle(VerifyCommand request, CancellationToken cancellationToken)
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

        await VerifyMonoAsync(request.ChatId, user.MonoToken, cancellationToken);
        await VerifyNotionAsync(request.ChatId, user.NotionToken, cancellationToken);
    }

    private async Task VerifyMonoAsync(long clientId, string token, CancellationToken cancellationToken)
    {
        try
        {
            await monobankClient.Personal.GetClientInfoAsync(token, cancellationToken);

            await client.SendTextMessageAsync(clientId, "Monobank account verified successfully.",
                cancellationToken: cancellationToken);
        }
        catch
        {
            await client.SendTextMessageAsync(clientId, "Monobank account verification failed.",
                cancellationToken: cancellationToken);
        }
    }
    
    private async Task VerifyNotionAsync(long clientId, string token, CancellationToken cancellationToken)
    {
        try
        {
            var accountsStorage = storageFactory.GetAccountsStorage(token);
            
            await accountsStorage.GetDatabaseIdAsync(cancellationToken);

            await client.SendTextMessageAsync(clientId, "Notion account verified successfully.",
                cancellationToken: cancellationToken);
        }
        catch
        {
            await client.SendTextMessageAsync(clientId, "Notion account verification failed.",
                cancellationToken: cancellationToken);
        }
    }
}