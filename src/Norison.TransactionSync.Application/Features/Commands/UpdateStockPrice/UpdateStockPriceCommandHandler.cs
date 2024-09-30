using System.Text.Json;

using Mediator;

using Norison.TransactionSync.Application.Models;
using Norison.TransactionSync.Application.Services.Messages;
using Norison.TransactionSync.Application.Services.Stocks;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Storages;
using Norison.TransactionSync.Persistence.Storages.Models;

using Notion.Client;

namespace Norison.TransactionSync.Application.Features.Commands.UpdateStockPrice;

public class UpdateStockPriceCommandHandler(
    IUsersService usersService,
    IStorageFactory storageFactory,
    IStocksService stocksService,
    IMessagesService messagesService) : ICommandHandler<UpdateStockPriceCommand>
{
    public async ValueTask<Unit> Handle(UpdateStockPriceCommand command, CancellationToken cancellationToken)
    {
        var users = await usersService.GetAllUsersAsync(cancellationToken);

        foreach (var user in users)
        {
            try
            {
                await UpdateUserStocksAsync(user, cancellationToken);
            }
            catch
            {
                // Ignore
            }
        }

        return Unit.Value;
    }

    private async Task UpdateUserStocksAsync(UserDbModel user, CancellationToken cancellationToken)
    {
        var userInfo = JsonSerializer.Deserialize<UserInfo>(user.Data)!;

        var accountsStorage = storageFactory.GetAccountsStorage(user.NotionToken);

        var parameters = new DatabasesQueryParameters
        {
            Filter = new CheckboxFilter(nameof(AccountDbModel.Ticker), true)
        };

        var accounts = await accountsStorage.GetAllAsync(userInfo.AccountsDatabaseId, parameters, cancellationToken);

        foreach (var account in accounts)
        {
            if (string.IsNullOrEmpty(account.Name))
            {
                continue;
            }

            try
            {
                var previousPrice = account.Price;
                var symbolPrice = await stocksService.GetSymbolPriceAsync(account.Name);

                if (previousPrice == symbolPrice)
                {
                    continue;
                }
                
                account.Price = symbolPrice;
                await accountsStorage.UpdateAsync(userInfo.AccountsDatabaseId, account, cancellationToken);
                await messagesService.SendMessageAsync(user.ChatId,
                    $"{account.Name} price updated from {previousPrice} to {symbolPrice}", cancellationToken);
            }
            catch
            {
                // Ignore
            }
        }
    }
}