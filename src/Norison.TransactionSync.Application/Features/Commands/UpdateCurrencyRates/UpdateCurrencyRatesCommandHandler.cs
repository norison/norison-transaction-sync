using System.Text.Json;

using Mediator;

using Monobank.Client;

using Norison.TransactionSync.Application.Models;
using Norison.TransactionSync.Application.Services.Messages;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

namespace Norison.TransactionSync.Application.Features.Commands.UpdateCurrencyRates;

public class UpdateCurrencyRatesCommandHandler(
    IUsersService usersService,
    IStorageFactory storageFactory,
    IMonobankClient monobankClient,
    IMessagesService messagesService) : ICommandHandler<UpdateCurrencyRatesCommand>
{
    private const int UsdCurrencyCode = 840;
    private const int EurCurrencyCode = 978;
    private const int UahCurrencyCode = 980;

    public async ValueTask<Unit> Handle(UpdateCurrencyRatesCommand command, CancellationToken cancellationToken)
    {
        var users = await usersService.GetAllUsersAsync(cancellationToken);

        if (users.Length == 0)
        {
            return Unit.Value;
        }

        var currencies = await monobankClient.Bank.GetCurrenciesAsync(cancellationToken);

        var eur = currencies.First(x => x is { CurrencyCodeA: EurCurrencyCode, CurrencyCodeB: UahCurrencyCode });
        var usd = currencies.First(x => x is { CurrencyCodeA: UsdCurrencyCode, CurrencyCodeB: UahCurrencyCode });

        var eurRate = Convert.ToDecimal(eur.RateSell);
        var usdRate = Convert.ToDecimal(usd.RateSell);

        foreach (var user in users)
        {
            try
            {
                var userInfo = JsonSerializer.Deserialize<UserInfo>(user.Data)!;
                var currenciesStorage = storageFactory.GetCurrenciesStorage(user.NotionToken);

                var queryParameters = new DatabasesQueryParameters
                {
                    Filter = new CompoundFilter([
                        new TitleFilter(nameof(CurrencyDbModel.Name), "EUR"),
                        new TitleFilter(nameof(CurrencyDbModel.Name), "USD")
                    ])
                };

                var currencyModels =
                    await currenciesStorage.GetAllAsync(userInfo.CurrenciesDatabaseId, queryParameters,
                        cancellationToken);

                var eurModel = currencyModels.First(x => x.Name == "EUR");
                var usdModel = currencyModels.First(x => x.Name == "USD");

                eurModel.Leveler = eurRate;
                usdModel.Leveler = usdRate;

                await Task.WhenAll(
                    currenciesStorage.UpdateAsync(userInfo.CurrenciesDatabaseId, eurModel, cancellationToken),
                    currenciesStorage.UpdateAsync(userInfo.CurrenciesDatabaseId, usdModel, cancellationToken));

                await messagesService.SendMessageAsync(user.ChatId,
                    $"Currency rates updated: EUR - {eurRate}, USD - {usdRate}", cancellationToken);
            }
            catch
            {
                // ignored
            }
        }

        return Unit.Value;
    }
}