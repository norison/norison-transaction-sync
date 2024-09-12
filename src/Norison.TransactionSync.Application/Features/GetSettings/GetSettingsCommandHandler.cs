using MediatR;

using Microsoft.Extensions.Options;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Persistence.Storages;

using Notion.Client;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Features.GetSettings;

public class GetSettingsCommandHandler(
    IStorageFactory storageFactory,
    ITelegramBotClient client,
    IOptions<NotionOptions> options) : IRequestHandler<GetSettingsCommand>
{
    public async Task Handle(GetSettingsCommand request, CancellationToken cancellationToken)
    {
        var usersStorage = storageFactory.GetUsersStorage();

        var parameters = new DatabasesQueryParameters { Filter = new NumberFilter("ChatId", request.ChatId) };
        var user = await usersStorage.GetFirstAsync(options.Value.NotionUsersDatabaseId, parameters, cancellationToken);

        if (user is null)
        {
            await client.SendTextMessageAsync(request.ChatId, "User not found", cancellationToken: cancellationToken);
            return;
        }

        var message = $"Notion token: {user.NotionToken}\n" +
                      $"Mono token: {user.MonoToken}\n" +
                      $"Mono account id: {user.MonoAccountId}\n" +
                      $"Accounts database id: {user.AccountsDatabaseId}\n" +
                      $"Budgets database id: {user.BudgetsDatabaseId}\n" +
                      $"Transactions database id: {user.TransactionsDatabaseId}";

        await client.SendTextMessageAsync(request.ChatId, message, cancellationToken: cancellationToken);
    }
}