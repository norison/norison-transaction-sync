using System.Text.RegularExpressions;

using MediatR;

using Microsoft.Extensions.Options;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Features.SetSettings;

public class SetSettingsCommandHandler(
    IStorageFactory storageFactory,
    ITelegramBotClient client,
    IOptions<NotionOptions> options) : IRequestHandler<SetSettingsCommand>
{
    public async Task Handle(SetSettingsCommand request, CancellationToken cancellationToken)
    {
        const string pattern = @"^1\.\s.+\r?\n2\.\s.+\r?\n3\.\s.+$";
        var isValid = Regex.IsMatch(request.Text, pattern, RegexOptions.Multiline);

        if (!isValid)
        {
            await client.SendTextMessageAsync(request.ChatId, "Invalid settings format.",
                cancellationToken: cancellationToken);
            return;
        }

        var lines = request.Text.Split("\n");
        var notionToken = lines[0].Split(".")[1].Trim();
        var monoToken = lines[1].Split(".")[1].Trim();
        var monoAccountName = lines[2].Split(".")[1].Trim();

        var userStorage = storageFactory.GetUsersStorage();
        var accountsStorage = storageFactory.GetAccountsStorage(notionToken);
        var budgetsStorage = storageFactory.GetBudgetsStorage(notionToken);
        var transactionsStorage = storageFactory.GetTransactionsStorage(notionToken);

        var accountsDatabaseIdTask = accountsStorage.GetDatabaseIdAsync(cancellationToken);
        var budgetsDatabaseIdTask = budgetsStorage.GetDatabaseIdAsync(cancellationToken);
        var transactionsDatabaseIdTask = transactionsStorage.GetDatabaseIdAsync(cancellationToken);

        await Task.WhenAll(accountsDatabaseIdTask, budgetsDatabaseIdTask, transactionsDatabaseIdTask);

        var usersDatabaseId = options.Value.NotionUsersDatabaseId;
        var accountsDatabaseId = accountsDatabaseIdTask.Result;
        var budgetsDatabaseId = budgetsDatabaseIdTask.Result;
        var transactionsDatabaseId = transactionsDatabaseIdTask.Result;

        var parameters = new DatabasesQueryParameters { Filter = new NumberFilter("ChatId", request.ChatId) };
        var user = await userStorage.GetFirstAsync(usersDatabaseId, parameters, cancellationToken);

        var monoAccount = await accountsStorage.GetFirstAsync(accountsDatabaseId,
            new DatabasesQueryParameters { Filter = new TitleFilter("Name", monoAccountName) }, cancellationToken);

        if (monoAccount is null)
        {
            await client.SendTextMessageAsync(request.ChatId, "Mono account not found.",
                cancellationToken: cancellationToken);
            return;
        }

        if (user is not null)
        {
            user.NotionToken = notionToken;
            user.MonoToken = monoToken;
            user.AccountsDatabaseId = accountsDatabaseId;
            user.BudgetsDatabaseId = budgetsDatabaseId;
            user.TransactionsDatabaseId = transactionsDatabaseId;
            user.MonoAccountId = monoAccount.Id!;
            await userStorage.UpdateAsync(usersDatabaseId, user, cancellationToken);
        }
        else
        {
            var userDbModel = new UserDbModel
            {
                Username = request.Username,
                ChatId = request.ChatId,
                NotionToken = notionToken,
                MonoToken = monoToken,
                AccountsDatabaseId = accountsDatabaseId,
                BudgetsDatabaseId = budgetsDatabaseId,
                TransactionsDatabaseId = transactionsDatabaseId,
                MonoAccountId = monoAccount.Id!
            };

            await userStorage.AddAsync(usersDatabaseId, userDbModel, cancellationToken);
        }


        await client.SendTextMessageAsync(request.ChatId, "Settings saved successfully.",
            cancellationToken: cancellationToken);
    }
}