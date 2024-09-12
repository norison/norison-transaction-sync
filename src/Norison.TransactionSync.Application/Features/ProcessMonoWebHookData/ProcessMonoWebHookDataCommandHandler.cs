using System.Text.Json;

using MediatR;

using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Norison.TransactionSync.Application.Features.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommandHandler(IStorageFactory storageFactory, ITelegramBotClient client)
    : IRequestHandler<ProcessMonoWebHookDataCommand>
{
    public async Task Handle(ProcessMonoWebHookDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var statement = request.WebHookData.StatementItem;

            await client.SendTextMessageAsync(request.ChatId, JsonSerializer.Serialize(statement),
                cancellationToken: cancellationToken);

            return;

            var userStorage = storageFactory.GetUsersStorage();

            var user = await userStorage.GetFirstAsync(
                new DatabasesQueryParameters { Filter = new NumberFilter("ChatId", request.ChatId) },
                cancellationToken);

            if (user is null)
            {
                return;
            }

            var accountsStorage = storageFactory.GetAccountsStorage(user.NotionToken);
            var transactionsStorage = storageFactory.GetTransactionsStorage(user.NotionToken);
            var budgetsStorage = storageFactory.GetBudgetsStorage(user.NotionToken);

            var getAccountTask = GetAccountAsync(accountsStorage, user.MonoAccountName, cancellationToken);
            var getBudgetTask = budgetsStorage.GetFirstAsync(new DatabasesQueryParameters { PageSize = 1 },
                cancellationToken);

            await Task.WhenAll(getAccountTask, getBudgetTask);
            (AccountDbModel account, BudgetDbModel? budget) = (getAccountTask.Result, getBudgetTask.Result);

            var type = statement.Amount > 0 ? TransactionType.Income : TransactionType.Expense;

            var amount = Math.Abs(statement.Amount / 100);

            var newTransaction = new TransactionDbModel
            {
                Name = statement.Description,
                Type = type,
                Date = statement.Time,
                AmountFrom = type == TransactionType.Expense ? amount : null,
                AmountTo = type == TransactionType.Income ? amount : null,
                Notes = statement.Comment,
                AccountFromIds = type == TransactionType.Expense ? [account.Id!] : [],
                AccountToIds = type == TransactionType.Income ? [account.Id!] : [],
                CategoryIds = [],
                BudgetIds = budget is null ? [] : [budget.Id!]
            };

            await transactionsStorage.AddAsync(newTransaction, cancellationToken);

            await client.SendTextMessageAsync(request.ChatId,
                $"Transaction '{statement.Description}' was added successfully.", cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            await client.SendTextMessageAsync(request.ChatId,
                $"Transaction was not added. Error: {exception.Message}",
                cancellationToken: cancellationToken);
        }
    }

    private static async Task<AccountDbModel> GetAccountAsync(IStorage<AccountDbModel> storage, string accountName,
        CancellationToken cancellationToken)
    {
        var account = await storage.GetFirstAsync(
            new DatabasesQueryParameters { Filter = new TitleFilter("Name", accountName) },
            cancellationToken);

        if (account is null)
        {
            throw new InvalidOperationException("Account not found.");
        }

        return account;
    }
}