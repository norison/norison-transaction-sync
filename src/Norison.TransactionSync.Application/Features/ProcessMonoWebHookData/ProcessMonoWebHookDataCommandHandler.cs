using System.Text.Json;

using MediatR;

using Microsoft.Extensions.Options;

using Norison.TransactionSync.Application.Models;
using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Features.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommandHandler(
    IStorageFactory storageFactory,
    ITelegramBotClient telegramBotClient,
    IOptions<NotionOptions> options) : IRequestHandler<ProcessMonoWebHookDataCommand>
{
    public async Task Handle(ProcessMonoWebHookDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var statement = request.WebHookData.StatementItem;

            var userStorage = storageFactory.GetUsersStorage();

            var user = await userStorage.GetFirstAsync(
                options.Value.NotionUsersDatabaseId,
                new DatabasesQueryParameters { Filter = new NumberFilter("ChatId", request.ChatId), PageSize = 1 },
                cancellationToken);

            if (user is null)
            {
                return;
            }

            var userInfo = JsonSerializer.Deserialize<UserInfo>(user.Data)!;

            var transactionsStorage = storageFactory.GetTransactionsStorage(user.NotionToken);
            var budgetsStorage = storageFactory.GetBudgetsStorage(user.NotionToken);

            var budget = await budgetsStorage.GetFirstAsync(userInfo.BudgetsDatabaseId,
                new DatabasesQueryParameters { PageSize = 1 }, cancellationToken);

            var type = statement.Amount > 0 ? TransactionType.Income : TransactionType.Expense;

            var amount = Math.Abs(statement.Amount) / 100m;

            var newTransaction = new TransactionDbModel
            {
                Name = statement.Description,
                Type = type,
                Date = statement.Time,
                AmountFrom = type == TransactionType.Expense ? amount : null,
                AmountTo = type == TransactionType.Income ? amount : null,
                Notes = statement.Comment,
                AccountFromIds = type == TransactionType.Expense ? [userInfo.MonoAccountId] : [],
                AccountToIds = type == TransactionType.Income ? [userInfo.MonoAccountId] : [],
                CategoryIds = [],
                BudgetIds = budget is null ? [] : [budget.Id!]
            };

            await transactionsStorage.AddAsync(userInfo.TransactionsDatabaseId, newTransaction, cancellationToken);

            await telegramBotClient.SendTextMessageAsync(request.ChatId,
                $"Transaction '{statement.Description}' was added successfully.", cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            await telegramBotClient.SendTextMessageAsync(request.ChatId,
                $"Transaction was not added. Error: {exception.Message}",
                cancellationToken: cancellationToken);
        }
    }
}