using System.Text.Json;

using Microsoft.Extensions.Options;

using Monobank.Client;

using Norison.TransactionSync.Application.Models;
using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Services.UserInfos;

public class UserInfosService(
    IStorageFactory storageFactory,
    ITelegramBotClient telegramBotClient,
    IMonobankClient monobankClient,
    IOptions<NotionOptions> options) : IUserInfosService
{
    public async Task PopulateUserAsync(
        long chatId,
        string username,
        string monoToken,
        string notionToken,
        string monoAccountName,
        CancellationToken cancellationToken)
    {
        var userStorage = storageFactory.GetUsersStorage();
        var accountsStorage = storageFactory.GetAccountsStorage(notionToken);
        var budgetsStorage = storageFactory.GetBudgetsStorage(notionToken);
        var transactionsStorage = storageFactory.GetTransactionsStorage(notionToken);
        var categoriesStorage = storageFactory.GetCategoriesStorage(notionToken);
        var currenciesStorage = storageFactory.GetCurrenciesStorage(notionToken);
        var automationsStorage = storageFactory.GetAutomationsStorage(notionToken);

        var accountsDatabaseIdTask = accountsStorage.GetDatabaseIdAsync(cancellationToken);
        var budgetsDatabaseIdTask = budgetsStorage.GetDatabaseIdAsync(cancellationToken);
        var transactionsDatabaseIdTask = transactionsStorage.GetDatabaseIdAsync(cancellationToken);
        var categoriesDatabaseIdTask = categoriesStorage.GetDatabaseIdAsync(cancellationToken);
        var currenciesDatabaseIdTask = currenciesStorage.GetDatabaseIdAsync(cancellationToken);
        var automationsDatabaseIdTask = automationsStorage.GetDatabaseIdAsync(cancellationToken);

        await Task.WhenAll(accountsDatabaseIdTask,
            budgetsDatabaseIdTask,
            transactionsDatabaseIdTask,
            categoriesDatabaseIdTask,
            currenciesDatabaseIdTask,
            automationsDatabaseIdTask);

        var usersDatabaseId = options.Value.NotionUsersDatabaseId;
        var accountsDatabaseId = accountsDatabaseIdTask.Result;
        var budgetsDatabaseId = budgetsDatabaseIdTask.Result;
        var transactionsDatabaseId = transactionsDatabaseIdTask.Result;
        var categoriesDatabaseId = categoriesDatabaseIdTask.Result;
        var currenciesDatabaseId = currenciesDatabaseIdTask.Result;
        var automationsDatabaseId = automationsDatabaseIdTask.Result;

        var user = await GetUserAsync(chatId, cancellationToken);

        var monoAccount = await accountsStorage.GetFirstAsync(accountsDatabaseId,
            new DatabasesQueryParameters { Filter = new TitleFilter("Name", monoAccountName) }, cancellationToken);

        if (monoAccount is null)
        {
            await telegramBotClient.SendTextMessageAsync(chatId, "Account not found.",
                cancellationToken: cancellationToken);
            return;
        }

        var monoBankAccount = await monobankClient.Personal.GetClientInfoAsync(monoToken, cancellationToken);

        var monoBankAccountInfo =
            monoBankAccount.Accounts.FirstOrDefault(x => x is { Type: AccountType.Black, CurrencyCode: 980 });

        if (monoBankAccountInfo is null)
        {
            await telegramBotClient.SendTextMessageAsync(chatId, "Account in bank not found.",
                cancellationToken: cancellationToken);
            return;
        }

        var userInfo = new UserInfo
        {
            MonoAccountBankId = monoBankAccountInfo.Id,
            MonoAccountId = monoAccount.Id!,
            AccountsDatabaseId = accountsDatabaseId,
            BudgetsDatabaseId = budgetsDatabaseId,
            TransactionsDatabaseId = transactionsDatabaseId,
            CategoriesDatabaseId = categoriesDatabaseId,
            CurrenciesDatabaseId = currenciesDatabaseId,
            AutomationsDatabaseId = automationsDatabaseId
        };

        var userInfoJson = JsonSerializer.Serialize(userInfo);

        if (user is null)
        {
            await userStorage.AddAsync(usersDatabaseId,
                new UserDbModel
                {
                    Username = username,
                    ChatId = chatId,
                    NotionToken = notionToken,
                    MonoToken = monoToken,
                    MonoAccountName = monoAccountName,
                    Data = userInfoJson
                }, cancellationToken);
        }
        else
        {
            user.Username = username;
            user.ChatId = chatId;
            user.NotionToken = notionToken;
            user.MonoToken = monoToken;
            user.MonoAccountName = monoAccountName;
            user.Data = userInfoJson;

            await userStorage.UpdateAsync(usersDatabaseId, user, cancellationToken);
        }
    }

    public async Task RefreshUserAsync(long chatId, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(chatId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        await PopulateUserAsync(chatId, user.Username, user.MonoToken, user.NotionToken, user.MonoAccountName,
            cancellationToken);
    }

    public async Task<UserDbModel?> GetUserAsync(long chatId, CancellationToken cancellationToken)
    {
        var userStorage = storageFactory.GetUsersStorage();

        return await userStorage.GetFirstAsync(options.Value.NotionUsersDatabaseId,
            new DatabasesQueryParameters { Filter = new NumberFilter("ChatId", chatId) }, cancellationToken);
    }
}