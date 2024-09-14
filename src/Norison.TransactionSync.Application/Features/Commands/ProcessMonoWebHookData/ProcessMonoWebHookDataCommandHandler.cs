using System.Text.Json;

using MediatR;

using Monobank.Client;

using Norison.TransactionSync.Application.Models;
using Norison.TransactionSync.Application.Services.Logs;
using Norison.TransactionSync.Application.Services.Messages;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

namespace Norison.TransactionSync.Application.Features.Commands.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommandHandler(
    IStorageFactory storageFactory,
    IUsersService usersService,
    IMessagesService messagesService,
    ILogsService logsService) : IRequestHandler<ProcessMonoWebHookDataCommand>
{
    public async Task Handle(ProcessMonoWebHookDataCommand request, CancellationToken cancellationToken)
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
                return;
            }

            try
            {
                await HandleInternalAsync(user, userInfo, request.WebHookData.StatementItem, cancellationToken);
            }
            catch (Exception exception)
            {
                await logsService.LogTransactionErrorAsync(user.Username, request.WebHookData.StatementItem, exception,
                    cancellationToken);
                throw;
            }

            await messagesService.SendMessageAsync(request.ChatId,
                "Monobank webhook data was processed successfully.",
                cancellationToken);
        }
        catch (Exception exception)
        {
            await messagesService.SendMessageAsync(request.ChatId,
                "Failed to process Monobank webhook data. Error: " + exception.Message,
                cancellationToken);
        }
    }

    private async Task HandleInternalAsync(
        UserDbModel user, UserInfo userInfo, Statement statement, CancellationToken cancellationToken)
    {
        var transactionsStorage = storageFactory.GetTransactionsStorage(user.NotionToken);
        var automationsStorage = storageFactory.GetAutomationsStorage(user.NotionToken);

        var automations =
            await automationsStorage.GetAllAsync(userInfo.AutomationsDatabaseId, cancellationToken: cancellationToken);

        var automation = FindAutomationForStatement(automations, statement);

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

        var lastBudgetId = await GetLastBudgetIdAsync(user, userInfo, cancellationToken);

        var newTransaction = new TransactionDbModel
        {
            IconUrl = "https://www.notion.so/icons/receipt_gray.svg",
            Name = description,
            Type = type,
            Date = statement.Time,
            AmountFrom = amountFrom,
            AmountTo = amountTo,
            Notes = statement.Comment,
            AccountFromIds = accountFromId is null ? [] : [accountFromId],
            AccountToIds = accountToId is null ? [] : [accountToId],
            CategoryIds = categoryId is null ? [] : [categoryId],
            BudgetIds = lastBudgetId is null ? [] : [lastBudgetId]
        };

        await transactionsStorage.AddAsync(userInfo.TransactionsDatabaseId, newTransaction, cancellationToken);

        await LogTransactionAsync(user.Username, statement, newTransaction, cancellationToken);
    }

    private async Task<string?> GetLastBudgetIdAsync(
        UserDbModel user, UserInfo userInfo, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetBudgetsStorage(user.NotionToken);

        var parameters = new DatabasesQueryParameters { PageSize = 1 };
        var budget = await storage.GetFirstAsync(userInfo.BudgetsDatabaseId, parameters, cancellationToken);
        return budget?.Id;
    }

    private static AutomationsDbModel? FindAutomationForStatement(AutomationsDbModel[] automations, Statement statement)
    {
        var validAutomations = automations
            .Where(x => !string.IsNullOrEmpty(x.DescriptionIs) || !string.IsNullOrEmpty(x.DescriptionContains))
            .ToArray();

        foreach (var automation in validAutomations)
        {
            if (!string.IsNullOrEmpty(automation.DescriptionIs))
            {
                if (string.Equals(statement.Description, automation.DescriptionIs, StringComparison.OrdinalIgnoreCase))
                {
                    return automation;
                }
            }

            if (!string.IsNullOrEmpty(automation.DescriptionContains))
            {
                if (statement.Description.Contains(automation.DescriptionContains, StringComparison.OrdinalIgnoreCase))
                {
                    return automation;
                }
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
            await logsService.LogTransactionAsync(username, statement, transaction, cancellationToken);
        }
        catch
        {
            // do nothing
        }
    }
}