using System.Text.Json;

using Mediator;

using Monobank.Client;

using Norison.TransactionSync.Application.Models;
using Norison.TransactionSync.Application.Services.Journal;
using Norison.TransactionSync.Application.Services.Messages;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Enums;
using Norison.TransactionSync.Persistence.Models;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

namespace Norison.TransactionSync.Application.Features.Commands.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommandHandler(
    IStorageFactory storageFactory,
    IUsersService usersService,
    IMessagesService messagesService,
    IJournalService journalService) : ICommandHandler<ProcessMonoWebHookDataCommand>
{
    public async ValueTask<Unit> Handle(ProcessMonoWebHookDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await usersService.GetUserByChatIdAsync(request.ChatId, cancellationToken);

            if (user is null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var userInfo = JsonSerializer.Deserialize<UserInfo>(user.Data)!;

            if (request.WebHookData.Account != userInfo.MonoAccountBankId)
            {
                return Unit.Value;
            }

            try
            {
                await HandleInternalAsync(user, userInfo, request.WebHookData.StatementItem, cancellationToken);
            }
            catch (Exception exception)
            {
                await journalService.LogTransactionErrorAsync(user.Username, request.WebHookData.StatementItem,
                    exception,
                    cancellationToken);
                throw;
            }

            await messagesService.SendMessageAsync(request.ChatId,
                $"Monobank transaction processed: {request.WebHookData.StatementItem.Description}",
                cancellationToken);
        }
        catch (Exception exception)
        {
            await messagesService.SendMessageAsync(request.ChatId,
                "Failed to process Monobank transaction. Error: " + exception.Message,
                cancellationToken);
        }

        return Unit.Value;
    }

    private async Task HandleInternalAsync(
        UserDbModel user, UserInfo userInfo, Statement statement, CancellationToken cancellationToken)
    {
        var transactionsStorage = storageFactory.GetTransactionsStorage(user.NotionToken);

        var automationTask = GetAutomationAsync(user.NotionToken, statement.Description,
            userInfo.AutomationsDatabaseId, cancellationToken);

        var budgetIdTask =
            GetBudgetIdAsync(user.NotionToken, userInfo.BudgetsDatabaseId, statement.Time, cancellationToken);

        await Task.WhenAll(automationTask, budgetIdTask);

        (AutomationsDbModel? automation, string? budgetId) = (await automationTask, await budgetIdTask);

        var description = string.IsNullOrEmpty(automation?.DescriptionOverride)
            ? statement.Description
            : automation.DescriptionOverride;

        var categoryId = automation?.CategoryIds.FirstOrDefault();

        var type = automation?.TransactionType ??
                   (statement.Amount > 0 ? TransactionType.Income : TransactionType.Expense);

        var accountFromId = statement.Amount < 0 ? userInfo.MonoAccountId : automation?.AccountFromIds.FirstOrDefault();
        var accountToId = statement.Amount > 0 ? userInfo.MonoAccountId : automation?.AccountToIds.FirstOrDefault();

        var amount = Math.Abs(statement.Amount) / 100m;
        var amountFrom = statement.Amount < 0 ? amount : (decimal?)null;
        var amountTo = statement.Amount > 0 ? amount : (decimal?)null;

        var newTransaction = new TransactionDbModel
        {
            IconUrl = "https://www.notion.so/icons/receipt_gray.svg",
            Name = description,
            Type = type,
            Date = new DateRange { StartDateTime = statement.Time },
            AmountFrom = amountFrom,
            AmountTo = amountTo,
            Notes = statement.Comment,
            AccountFromIds = accountFromId is null ? [] : [accountFromId],
            AccountToIds = accountToId is null ? [] : [accountToId],
            CategoryIds = categoryId is null ? [] : [categoryId],
            BudgetIds = budgetId is null ? [] : [budgetId]
        };

        await transactionsStorage.AddAsync(userInfo.TransactionsDatabaseId, newTransaction, cancellationToken);

        await LogTransactionAsync(user.Username, statement, newTransaction, cancellationToken);
    }

    private async Task<string?> GetBudgetIdAsync(
        string token, string databaseId, DateTime time, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetBudgetsStorage(token);

        var parameters = new DatabasesQueryParameters
        {
            PageSize = 1,
            Filter = new DateFilter("Date Range", onOrBefore: time),
            Sorts = [new Sort { Property = "Date Range", Direction = Direction.Descending }]
        };
        var budget = await storage.GetFirstAsync(databaseId, parameters, cancellationToken);
        return budget?.Id;
    }

    private async Task<AutomationsDbModel?> GetAutomationAsync(
        string token, string description, string databaseId, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetAutomationsStorage(token);

        var parameters = new DatabasesQueryParameters
        {
            Filter = new CompoundFilter(or:
            [
                new RichTextFilter("Description Is", isNotEmpty: true),
                new RichTextFilter("Description Contains", isNotEmpty: true)
            ])
        };

        var automations = await storage.GetAllAsync(databaseId, parameters, cancellationToken);

        foreach (var automation in automations)
        {
            if (!string.IsNullOrEmpty(automation.DescriptionIs) && string.Equals(description,
                    automation.DescriptionIs, StringComparison.OrdinalIgnoreCase))
            {
                return automation;
            }

            if (!string.IsNullOrEmpty(automation.DescriptionContains) &&
                description.Contains(automation.DescriptionContains, StringComparison.OrdinalIgnoreCase))
            {
                return automation;
            }
        }

        return null;
    }

    private async Task LogTransactionAsync(
        string username, Statement statement, TransactionDbModel transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await journalService.LogTransactionAsync(username, statement, transaction, cancellationToken);
        }
        catch
        {
            // do nothing
        }
    }
}