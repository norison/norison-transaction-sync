using System.Text.Json;
using System.Text.RegularExpressions;

using Mediator;

using Monobank.Client;

using Norison.TransactionSync.Application.Models;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

namespace Norison.TransactionSync.Application.Features.Commands.SetSettings;

public partial class SetSettingsCommandHandler(
    IStorageFactory storageFactory,
    IUsersService usersService,
    IMonobankClient monobankClient) : ICommandHandler<SetSettingsCommand>
{
    public async ValueTask<Unit> Handle(SetSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!SettingsRegex().IsMatch(request.Text))
        {
            throw new InvalidOperationException("Invalid settings format.");
        }

        var user = new UserDbModel { ChatId = request.ChatId, Username = request.Username };

        var lines = request.Text.Split("\n");
        user.NotionToken = lines[0].Split(".")[1].Trim();
        user.MonoToken = lines[1].Split(".")[1].Trim();
        user.MonoAccountName = lines[2].Split(".")[1].Trim();

        await PopulateUserDataAsync(user, cancellationToken);

        await usersService.SetUserAsync(user, cancellationToken);

        return Unit.Value;
    }

    private async Task PopulateUserDataAsync(UserDbModel user, CancellationToken cancellationToken)
    {
        var userInfo = new UserInfo();

        await Task.WhenAll(
            PopulateUserAccountDataAsync(userInfo, user, cancellationToken),
            PopulateUserBudgetDataAsync(userInfo, user, cancellationToken),
            PopulateUserTransactionsDataAsync(userInfo, user, cancellationToken),
            PopulateUserCategoriesDataAsync(userInfo, user, cancellationToken),
            PopulateUserCurrenciesDataAsync(userInfo, user, cancellationToken),
            PopulateUserAutomationsDataAsync(userInfo, user, cancellationToken));

        user.Data = JsonSerializer.Serialize(userInfo);
    }

    private async Task PopulateUserAccountDataAsync(
        UserInfo userInfo, UserDbModel user, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetAccountsStorage(user.NotionToken);

        var databaseId = await storage.GetDatabaseIdAsync(cancellationToken);

        var monoAccountQueryParameters = new DatabasesQueryParameters
        {
            Filter = new TitleFilter(nameof(AccountDbModel.Name), user.MonoAccountName)
        };
        var monoAccount = await storage.GetFirstAsync(databaseId, monoAccountQueryParameters, cancellationToken);

        if (monoAccount is null)
        {
            throw new InvalidOperationException("Mono account not found in notion.");
        }

        var monoClientInfo = await monobankClient.Personal.GetClientInfoAsync(user.MonoToken, cancellationToken);
        var monobankAccount =
            monoClientInfo.Accounts.FirstOrDefault(x => x is { Type: AccountType.Black, CurrencyCode: 980 });

        if (monobankAccount is null)
        {
            throw new InvalidOperationException("Account in bank not found.");
        }

        userInfo.AccountsDatabaseId = databaseId;
        userInfo.MonoAccountId = monoAccount.Id!;
        userInfo.MonoAccountBankId = monobankAccount.Id;
    }

    private async Task PopulateUserBudgetDataAsync(
        UserInfo userInfo, UserDbModel user, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetBudgetsStorage(user.NotionToken);
        var databaseId = await storage.GetDatabaseIdAsync(cancellationToken);
        userInfo.BudgetsDatabaseId = databaseId;
    }

    private async Task PopulateUserTransactionsDataAsync(
        UserInfo userInfo, UserDbModel user, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetTransactionsStorage(user.NotionToken);
        var databaseId = await storage.GetDatabaseIdAsync(cancellationToken);
        userInfo.TransactionsDatabaseId = databaseId;
    }

    private async Task PopulateUserCategoriesDataAsync(
        UserInfo userInfo, UserDbModel user, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetCategoriesStorage(user.NotionToken);
        var databaseId = await storage.GetDatabaseIdAsync(cancellationToken);
        userInfo.CategoriesDatabaseId = databaseId;
    }

    private async Task PopulateUserCurrenciesDataAsync(
        UserInfo userInfo, UserDbModel user, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetCurrenciesStorage(user.NotionToken);
        var databaseId = await storage.GetDatabaseIdAsync(cancellationToken);
        userInfo.CurrenciesDatabaseId = databaseId;
    }

    private async Task PopulateUserAutomationsDataAsync(
        UserInfo userInfo, UserDbModel user, CancellationToken cancellationToken)
    {
        var storage = storageFactory.GetAutomationsStorage(user.NotionToken);
        var databaseId = await storage.GetDatabaseIdAsync(cancellationToken);
        userInfo.AutomationsDatabaseId = databaseId;
    }

    [GeneratedRegex(@"^1\.\s.+\r?\n2\.\s.+\r?\n3\.\s.+$", RegexOptions.Multiline)]
    private static partial Regex SettingsRegex();
}